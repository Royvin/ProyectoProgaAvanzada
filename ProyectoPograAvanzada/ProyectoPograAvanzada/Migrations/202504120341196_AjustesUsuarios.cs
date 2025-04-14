namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjustesUsuarios : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Carrera");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Carrera", c => c.String());
        }
    }
}
