#!/bin/bash
set -e

APP_NAME="Sockethead"
RESOURCE_GROUP="appsvc_linux_centralus"
PROJECT="Sockethead.Web/Sockethead.Web.csproj"
PUBLISH_DIR="./publish"
ZIP_FILE="deploy.zip"

echo "==> Building and publishing..."
dotnet publish "$PROJECT" -c Release -o "$PUBLISH_DIR"

echo "==> Creating deployment package..."
rm -f "$ZIP_FILE"
cd "$PUBLISH_DIR"
zip -r "../$ZIP_FILE" .
cd ..

echo "==> Getting deployment credentials..."
CREDS=$(az webapp deployment list-publishing-credentials \
    --name "$APP_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --query "{user:publishingUserName, pass:publishingPassword}" \
    -o json)
USER=$(echo "$CREDS" | python3 -c "import json,sys; print(json.load(sys.stdin)['user'])")
PASS=$(echo "$CREDS" | python3 -c "import json,sys; print(json.load(sys.stdin)['pass'])")

echo "==> Deploying to Azure..."
STATUS=$(curl -s -w "%{http_code}" -o /dev/null \
    -X POST --data-binary "@$ZIP_FILE" \
    -H "Content-Type: application/zip" \
    "https://$USER:$PASS@${APP_NAME,,}.scm.azurewebsites.net/api/zipdeploy?isAsync=true")

if [ "$STATUS" = "202" ]; then
    echo "==> Deployment accepted. Waiting for completion..."
    for i in $(seq 1 30); do
        sleep 5
        RESULT=$(curl -s "https://$USER:$PASS@${APP_NAME,,}.scm.azurewebsites.net/api/deployments/latest")
        COMPLETE=$(echo "$RESULT" | python3 -c "import json,sys; print(json.load(sys.stdin).get('complete', False))")
        if [ "$COMPLETE" = "True" ]; then
            DEP_STATUS=$(echo "$RESULT" | python3 -c "import json,sys; print(json.load(sys.stdin).get('status', -1))")
            if [ "$DEP_STATUS" = "4" ]; then
                echo "==> Deployment succeeded!"
            else
                echo "==> Deployment finished with status: $DEP_STATUS"
            fi
            break
        fi
        echo "    Still deploying... (${i})"
    done
else
    echo "==> Deployment failed with HTTP status: $STATUS"
    exit 1
fi

echo "==> Cleaning up..."
rm -rf "$PUBLISH_DIR" "$ZIP_FILE"

echo "==> Done! Site: https://${APP_NAME,,}.azurewebsites.net"
