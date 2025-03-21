namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Carrito : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CarritoItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductoId = c.Int(nullable: false),
                        Nombre = c.String(),
                        Precio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cantidad = c.Int(nullable: false),
                        Carrito_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Carritoes", t => t.Carrito_Id)
                .Index(t => t.Carrito_Id);
            
            CreateTable(
                "dbo.Carritoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CarritoItems", "Carrito_Id", "dbo.Carritoes");
            DropIndex("dbo.CarritoItems", new[] { "Carrito_Id" });
            DropTable("dbo.Carritoes");
            DropTable("dbo.CarritoItems");
        }
    }
}
