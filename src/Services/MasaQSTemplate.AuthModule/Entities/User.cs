namespace MasaQSTemplate.AuthModule.Entities
{
    [SugarTable("Users")]
    public class User : FullEntity<Guid, Guid>
    {
        public string Account { get; set; }
        public string Password { get; set; }
        [SugarColumn(IsNullable = true)]
        public string UserName { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Avatar { get; set; }
        public bool Gender { get; set; }
        [SugarColumn(IsNullable = true, Length = 18)]
        public string IdCard { get; set; }
        [SugarColumn(IsNullable =true,Length = 11)]
        public string PhoneNumber { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Email { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Address { get; set; }
        [SugarColumn(IsNullable = true)]
        public string CompanyName { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Department { get; set; }
        public bool Enabled { get; set; }
    }
}
