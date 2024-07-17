using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyIp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TwoLetterCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ThreeLetterCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IpAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ip = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IpAddresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_TwoLetterCode",
                table: "Countries",
                column: "TwoLetterCode",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_IpAddresses_CountryId",
                table: "IpAddresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_IpAddresses_Ip",
                table: "IpAddresses",
                column: "Ip",
                unique: true)
                .Annotation("SqlServer:Clustered", false);
            
            var sql = @"
            CREATE PROCEDURE GetCountryReport
                @CountryCodes NVARCHAR(MAX) = NULL
            AS
            BEGIN
                SET NOCOUNT ON;

                DECLARE @SQL NVARCHAR(MAX);

                SET @SQL = '
                    SELECT 
                        c.Name AS CountryName,
                        COUNT(ip.Id) AS AddressesCount,
                        MAX(ip.UpdatedAt) AS LastAddressUpdated
                    FROM 
                        Countries c
                    LEFT JOIN 
                        IpAddresses ip ON c.Id = ip.CountryId';

                IF @CountryCodes IS NOT NULL AND @CountryCodes <> ''
                BEGIN
                    SET @SQL = @SQL + '
                        WHERE 
                            c.TwoLetterCode IN (''' + REPLACE(@CountryCodes, ',', ''',''') + ''')';
                END

                SET @SQL = @SQL + '
                    GROUP BY 
                        c.Name';

                EXEC sp_executesql @SQL;
            END";

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sql = "DROP PROCEDURE IF EXISTS GetCountryReport";
            migrationBuilder.Sql(sql);
            
            migrationBuilder.DropTable(
                name: "IpAddresses");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
