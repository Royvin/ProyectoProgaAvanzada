namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegistroActualizado : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "NombreCompleto", c => c.String());
            AddColumn("dbo.AspNetUsers", "Carrera", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Carrera");
            DropColumn("dbo.AspNetUsers", "NombreCompleto");
        }
    }
}
