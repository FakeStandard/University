using System;
using System.Collections.Generic;
using University.Models;

namespace University.DAL.IRepository
{
    public interface IStudentRepository : IDisposable
    {
        IEnumerable<Student> GetStudents();

        Student GetStudentByID(int? id);
        void InsertStudent(Student student);
        void UpdateStudent(Student student);
        void DeleteStudent(int? id);
        void Save();
        bool IsExists(int id);
    }
}
