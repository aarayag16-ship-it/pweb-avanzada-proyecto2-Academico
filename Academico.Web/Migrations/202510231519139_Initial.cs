namespace Academico.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cursoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(nullable: false, maxLength: 20),
                        Nombre = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Estudiantes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 50),
                        Apellidos = c.String(nullable: false, maxLength: 80),
                        Identificacion = c.String(nullable: false, maxLength: 25),
                        FechaNacimiento = c.DateTime(nullable: false),
                        Provincia = c.String(nullable: false),
                        Canton = c.String(nullable: false),
                        Distrito = c.String(nullable: false),
                        Correo = c.String(nullable: false, maxLength: 120),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Identificacion, unique: true)
                .Index(t => t.Correo, unique: true);
            
            CreateTable(
                "dbo.Evaluacions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EstudianteId = c.Int(nullable: false),
                        TerminoId = c.Int(nullable: false),
                        CursoId = c.Int(nullable: false),
                        Nota = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TipoParticipacion = c.String(nullable: false, maxLength: 50),
                        Estado = c.String(nullable: false, maxLength: 20),
                        Observaciones = c.String(maxLength: 500),
                        FechaRegistro = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cursoes", t => t.CursoId, cascadeDelete: true)
                .ForeignKey("dbo.Estudiantes", t => t.EstudianteId, cascadeDelete: true)
                .ForeignKey("dbo.Terminoes", t => t.TerminoId, cascadeDelete: true)
                .Index(t => new { t.EstudianteId, t.TerminoId, t.CursoId }, unique: true);
            
            CreateTable(
                "dbo.Terminoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 50),
                        FechaInicio = c.DateTime(nullable: false),
                        FechaFin = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MatriculaCursoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MatriculaId = c.Int(nullable: false),
                        CursoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cursoes", t => t.CursoId, cascadeDelete: true)
                .ForeignKey("dbo.Matriculas", t => t.MatriculaId, cascadeDelete: true)
                .Index(t => t.MatriculaId)
                .Index(t => t.CursoId);
            
            CreateTable(
                "dbo.Matriculas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EstudianteId = c.Int(nullable: false),
                        TerminoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estudiantes", t => t.EstudianteId, cascadeDelete: true)
                .ForeignKey("dbo.Terminoes", t => t.TerminoId, cascadeDelete: true)
                .Index(t => t.EstudianteId)
                .Index(t => t.TerminoId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Matriculas", "TerminoId", "dbo.Terminoes");
            DropForeignKey("dbo.Matriculas", "EstudianteId", "dbo.Estudiantes");
            DropForeignKey("dbo.MatriculaCursoes", "MatriculaId", "dbo.Matriculas");
            DropForeignKey("dbo.MatriculaCursoes", "CursoId", "dbo.Cursoes");
            DropForeignKey("dbo.Evaluacions", "TerminoId", "dbo.Terminoes");
            DropForeignKey("dbo.Evaluacions", "EstudianteId", "dbo.Estudiantes");
            DropForeignKey("dbo.Evaluacions", "CursoId", "dbo.Cursoes");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Matriculas", new[] { "TerminoId" });
            DropIndex("dbo.Matriculas", new[] { "EstudianteId" });
            DropIndex("dbo.MatriculaCursoes", new[] { "CursoId" });
            DropIndex("dbo.MatriculaCursoes", new[] { "MatriculaId" });
            DropIndex("dbo.Evaluacions", new[] { "EstudianteId", "TerminoId", "CursoId" });
            DropIndex("dbo.Estudiantes", new[] { "Correo" });
            DropIndex("dbo.Estudiantes", new[] { "Identificacion" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Matriculas");
            DropTable("dbo.MatriculaCursoes");
            DropTable("dbo.Terminoes");
            DropTable("dbo.Evaluacions");
            DropTable("dbo.Estudiantes");
            DropTable("dbo.Cursoes");
        }
    }
}
