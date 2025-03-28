namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Resenas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reseñas",
                c => new
                    {
                        IdReseña = c.Int(nullable: false, identity: true),
                        Titulo = c.String(nullable: false, maxLength: 200),
                        Descripcion = c.String(nullable: false, maxLength: 200),
                        IdProducto = c.Int(nullable: false),
                        Calificacion = c.Int(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IdReseña)
                .ForeignKey("dbo.Productoes", t => t.IdProducto, cascadeDelete: true)
                .Index(t => t.IdProducto);
            
            AddColumn("dbo.AspNetUsers", "NombreCompleto", c => c.String());
            AddColumn("dbo.AspNetUsers", "Carrera", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reseñas", "IdProducto", "dbo.Productoes");
            DropIndex("dbo.Reseñas", new[] { "IdProducto" });
            DropColumn("dbo.AspNetUsers", "Carrera");
            DropColumn("dbo.AspNetUsers", "NombreCompleto");
            DropTable("dbo.Reseñas");
        }
    }
}
