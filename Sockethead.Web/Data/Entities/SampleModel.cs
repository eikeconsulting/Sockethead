using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sockethead.Web.Data.Entities
{
    public enum SampleEnum
    {
        Zero, One, Two, Three, Four
    }

    public class SampleModel
    {
        [Display(Name = "Unique ID")]
        public int Id { get; set; } = NextId++;

        [DisplayName("First Name")]
        [Required(ErrorMessage = "Yo, we need a first name here")]
        [MaxLength(20, ErrorMessage = "First name is tooooo long...")]
        public string First { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(20)]
        public string Last { get; set; }

        public string JobTitle { get; set; }

        //[DataType(DataType.Date)]
        public DateTime RandomDate { get; set; } = GetRandomDate();

        public bool Flag { get; set; } = GetRandomBool();

        public SampleEnum SampleEnum { get; set; } = GetRandomEnum();

        public string FooBarBazBBBlah { get; set; }

        private static int NextId = 1001;

        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        private static DateTime GetRandomDate()
            => DateTime.UtcNow
                .AddDays(-Random.NextDouble() * 10000)
                .AddMinutes(Random.NextDouble() * 24 * 60);

        private static bool GetRandomBool() => Random.NextDouble() > 0.5;

        private static SampleEnum GetRandomEnum() => (SampleEnum)Random.Next((int)SampleEnum.Zero, (int)SampleEnum.Four + 1);

        public override string ToString() => $"Sample Data for {First} {Last}, {JobTitle}";
    }
}
