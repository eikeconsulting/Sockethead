namespace Sockethead.Test.Common.Models
{
    internal class TestModel
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }

        public TestModel(string prop1, string prop2)
        {
            Property1 = prop1;
            Property2 = prop2;
        }
    }
}