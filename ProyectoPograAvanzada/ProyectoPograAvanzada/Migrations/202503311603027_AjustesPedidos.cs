namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjustesPedidos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PedidoItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductoId = c.Int(nullable: false),
                        Nombre = c.String(),
                        Precio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cantidad = c.Int(nullable: false),
                        IdPedido = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pedidos", t => t.IdPedido, cascadeDelete: true)
                .Index(t => t.IdPedido);
            
            DropTable("dbo.DetallePedidos");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DetallePedidos",
                c => new
                    {
                        IdDetalle = c.Int(nullable: false, identity: true),
                        IdPedido = c.Int(nullable: false),
                        IdItem = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdDetalle);
            
            DropForeignKey("dbo.PedidoItems", "IdPedido", "dbo.Pedidos");
            DropIndex("dbo.PedidoItems", new[] { "IdPedido" });
            DropTable("dbo.PedidoItems");
        }
    }
}
