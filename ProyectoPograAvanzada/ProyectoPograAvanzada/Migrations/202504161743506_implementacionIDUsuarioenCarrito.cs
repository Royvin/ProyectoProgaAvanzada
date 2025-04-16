namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class implementacionIDUsuarioenCarrito : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carritoes", "UsuarioId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Carritoes", "UsuarioId");
            AddForeignKey("dbo.Carritoes", "UsuarioId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Carritoes", "UsuarioId", "dbo.AspNetUsers");
            DropIndex("dbo.Carritoes", new[] { "UsuarioId" });
            DropColumn("dbo.Carritoes", "UsuarioId");
        }
    }
}
