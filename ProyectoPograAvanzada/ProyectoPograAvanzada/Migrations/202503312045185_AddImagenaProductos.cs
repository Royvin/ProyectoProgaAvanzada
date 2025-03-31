namespace ProyectoPograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImagenaProductos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productoes", "Imagen", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Productoes", "Imagen");
        }
    }
}
