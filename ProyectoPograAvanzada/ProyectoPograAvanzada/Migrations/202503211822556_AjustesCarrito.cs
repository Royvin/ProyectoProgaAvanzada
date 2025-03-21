namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjustesCarrito : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CarritoItems", "Carrito_Id", "dbo.Carritoes");
            DropIndex("dbo.CarritoItems", new[] { "Carrito_Id" });
            RenameColumn(table: "dbo.CarritoItems", name: "Carrito_Id", newName: "CarritoId");
            AlterColumn("dbo.CarritoItems", "CarritoId", c => c.Int(nullable: false));
            CreateIndex("dbo.CarritoItems", "CarritoId");
            AddForeignKey("dbo.CarritoItems", "CarritoId", "dbo.Carritoes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CarritoItems", "CarritoId", "dbo.Carritoes");
            DropIndex("dbo.CarritoItems", new[] { "CarritoId" });
            AlterColumn("dbo.CarritoItems", "CarritoId", c => c.Int());
            RenameColumn(table: "dbo.CarritoItems", name: "CarritoId", newName: "Carrito_Id");
            CreateIndex("dbo.CarritoItems", "Carrito_Id");
            AddForeignKey("dbo.CarritoItems", "Carrito_Id", "dbo.Carritoes", "Id");
        }
    }
}
