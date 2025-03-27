namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PedidosYDetallesPedido : DbMigration
    {
        public override void Up()
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Pedidos");
            DropTable("dbo.DetallePedidos");
        }
    }
}
