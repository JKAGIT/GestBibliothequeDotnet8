using System.Collections.Generic;
using System.Reflection.Emit;

using GestBibliothequeDotnet8.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Net;
using System.Runtime.ConstrainedExecution;

namespace GestBibliothequeDotnet8.Donnee
{
    public class GestBibliothequeDbContext : DbContext
    {
        public GestBibliothequeDbContext(DbContextOptions<GestBibliothequeDbContext> options) : base(options)
        {
        }
        public DbSet<Livres> Livres { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Utilisateurs> Utilisateurs { get; set; }
        public DbSet<Usagers> Usagers { get; set; }
        public DbSet<Emprunts> Emprunts { get; set; }
        public DbSet<Retours> Retours { get; set; }
        public DbSet<Reservations> Reservations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Livres>()
            .HasOne(l => l.Categories)
            .WithMany(c => c.Livres)
                .HasForeignKey(l => l.IDCategorie)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Emprunts>()
                .HasOne(e => e.Usager)
                .WithMany(u => u.Emprunts)
                .HasForeignKey(e => e.IDUsager)
                .OnDelete(DeleteBehavior.Cascade);

            //OnDelete(DeleteBehavior.Restrict) : Un livre ne peut pas être supprimé s'il est emprunté.
            modelBuilder.Entity<Emprunts>()
                .HasOne(e => e.Livre)
                .WithMany(l => l.Emprunts)
                .HasForeignKey(e => e.IDLivre)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Retours>()
               .HasOne(r => r.Emprunt)
               .WithOne(e => e.Retours)
               .HasForeignKey<Retours>(r => r.IDEmprunt)
               .OnDelete(DeleteBehavior.Cascade);


            // Configurer la relation 1:1 entre Emprunts et Reservations
            modelBuilder.Entity<Emprunts>()
                .HasOne(e => e.Reservation)
                .WithOne(r => r.Emprunt)
                .HasForeignKey<Emprunts>(e => e.IDReservation)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurer la relation entre Livre et Reservation
            modelBuilder.Entity<Reservations>()
                .HasOne(r => r.Livre)
                .WithMany(l => l.Reservations)
                .HasForeignKey(r => r.IDLivre)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurer la relation entre Usager et Reservation
            modelBuilder.Entity<Reservations>()
                .HasOne(r => r.Usager)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.IDUsager)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}


