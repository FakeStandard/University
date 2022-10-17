using System;
using System.Collections.Generic;
using System.Linq;
using University.DAL.IRepository;
using University.Data;
using University.Models;
using Microsoft.EntityFrameworkCore;

namespace University.DAL.Repository
{
    public class StudentRepository : IStudentRepository, IDisposable
    {
        private SchoolContext context;

        public StudentRepository(SchoolContext context)
        {
            this.context = context;
        }

        public IEnumerable<Student> GetStudents()
        {
            return GetStudentsRaw().ToList();
        }

        public DbSet<Student> GetStudentsRaw()
        {
            return context.Students;
        }

        public Student GetByID(int? id)
        {
            return context.Students.Find(id);
        }

        public Student GetStudentByID(int? id)
        {
            return context.Students
                .Include(e => e.Enrollments)
                .ThenInclude(c => c.Course)
                .AsNoTracking() // 停用追蹤記憶體中的實體物件
                .FirstOrDefault(r => r.ID == id);
        }

        public void InsertStudent(Student student)
        {
            context.Students.Add(student);
        }

        public void UpdateStudent(Student student)
        {
            context.Entry(student).State = EntityState.Modified;
        }

        public void DeleteStudent(int? id)
        {
            Student student = context.Students.Find(id);
            context.Students.Remove(student);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public bool IsExists(int id)
        {
            return context.Students.Any(s => s.ID == id);
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
                if (disposing)
                    context.Dispose();

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
