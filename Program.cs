using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MFAsys
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FormLog());
            // Application.Run(new FormAdm());
            //Application.Run(new FormStu("1171002085"));
            //Application.Run(new FormStu("1171002086"));
            //Application.Run(new FormStu("1171002098"));
            //Application.Run(new FormStu("1171002091"));
            //Application.Run(new FormStu("1171002090"));
            //Application.Run(new FormTea("1001"));
            Application.Run(new FormTea("1003"));
        }
    }
}

/*数据库
Teacher:
    Id,
    Name,
    Rank,
    Password

Student:
    Id,
    Name,
    Class,
    Password,
    CroppedFace,
    FeatureIndex
                    
Course:
    Id,
    NameCourse,
    Credit,
    NameTeacher

RecordAttend:
    IdStudent,
    IdCourse,
    TimesCourse,
    StatusAbsence,
    SumAbsence
*/