namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fechaUltimaconexion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UltimaConexion", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UltimaConexion");
        }
    }
}
