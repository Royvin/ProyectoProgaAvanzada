namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PedidoDetallePedido : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DetallePedidoes", newName: "DetallePedidos");
            CreateTable(
                "dbo.Pedidos",
                c => new
                    {
                        IdPedido = c.Int(nullable: false, identity: true),
                        idCarrito = c.Int(nullable: false),
                        FechaCompra = c.DateTime(nullable: false),
                        Estado = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IdPedido);
            
            DropTable("dbo.Pedidoes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Pedidoes",
                c => new
                    {
                        IdPedido = c.Int(nullable: false, identity: true),
                        FechaCompra = c.DateTime(nullable: false),
                        IdCarrito = c.Int(nullable: false),
                        Estado = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IdPedido);
            
            DropTable("dbo.Pedidos");
            RenameTable(name: "dbo.DetallePedidos", newName: "DetallePedidoes");
        }
    }
}
