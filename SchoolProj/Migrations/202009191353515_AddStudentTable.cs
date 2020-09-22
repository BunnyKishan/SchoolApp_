namespace SchoolProj.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStudentTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Gender = c.Byte(nullable: false),
                        RollNo = c.String(),
                        Age = c.Long(nullable: false),
                        PhoneNo = c.String(maxLength: 15),
                        Email = c.String(nullable: false, maxLength: 50),
                        Address = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Students");
        }
    }
}
