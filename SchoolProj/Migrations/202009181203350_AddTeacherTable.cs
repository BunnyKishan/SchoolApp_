namespace SchoolProj.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTeacherTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Gender = c.Byte(nullable: false),
                        Department = c.Byte(nullable: false),
                        PhoneNo = c.String(maxLength: 15),
                        Email = c.String(nullable: false, maxLength: 50),
                        Address = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Teachers");
        }
    }
}
