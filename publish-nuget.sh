#!/bin/bash
set -e

PROJECT="Sockethead.Razor/Sockethead.Razor.csproj"
SOURCE="nuget.org"

# Get current version from csproj
VERSION=$(grep -oP '<Version>\K[^<]+' "$PROJECT")
echo "==> Current version: $VERSION"

# Get API key from NuGet config
API_KEY=$(python3 -c "
import xml.etree.ElementTree as ET
tree = ET.parse('$HOME/.nuget/NuGet/NuGet.Config')
for elem in tree.iter('add'):
    if elem.get('key') == 'ClearTextPassword':
        print(elem.get('value'))
        break
")

if [ -z "$API_KEY" ]; then
    echo "Error: No API key found in NuGet config."
    echo "Run: dotnet nuget update source nuget.org --username ignored --password YOUR_KEY --store-password-in-clear-text"
    exit 1
fi

echo "==> Building Release..."
dotnet build "$PROJECT" -c Release

NUPKG="Sockethead.Razor/bin/Release/Sockethead.Razor.${VERSION}.nupkg"
if [ ! -f "$NUPKG" ]; then
    echo "Error: Package not found at $NUPKG"
    exit 1
fi

echo "==> Publishing Sockethead.Razor v${VERSION} to nuget.org..."
read -p "    Continue? (y/N) " confirm
if [ "$confirm" != "y" ] && [ "$confirm" != "Y" ]; then
    echo "Aborted."
    exit 0
fi

dotnet nuget push "$NUPKG" --source "$SOURCE" --api-key "$API_KEY"

echo "==> Done! Package will appear at:"
echo "    https://www.nuget.org/packages/Sockethead.Razor/${VERSION}"
