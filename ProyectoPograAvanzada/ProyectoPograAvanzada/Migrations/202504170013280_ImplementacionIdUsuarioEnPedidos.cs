namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImplementacionIdUsuarioEnPedidos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pedidos", "UsuarioId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Pedidos", "UsuarioId");
            AddForeignKey("dbo.Pedidos", "UsuarioId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pedidos", "UsuarioId", "dbo.AspNetUsers");
            DropIndex("dbo.Pedidos", new[] { "UsuarioId" });
            DropColumn("dbo.Pedidos", "UsuarioId");
        }
    }
}
