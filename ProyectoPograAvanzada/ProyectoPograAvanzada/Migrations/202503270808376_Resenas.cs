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
                    })
                .PrimaryKey(t => t.IdReseña);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Reseñas");
        }
    }
}
