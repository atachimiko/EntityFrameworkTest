using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkTest
{
	class Program
	{
		static void Main(string[] args)
		{
			string personalDirectoryPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			AppDomain.CurrentDomain.SetData("DataDirectory", personalDirectoryPath);

#if SQLITE
			SqliteInitializeSql();
#else
			MyDbContext.MyDbContextInitialize();
#endif
			

			using (var @dbc = new MyDbContext())
			{
				var @p1 = new Person
				{
					Name = "Person1",
					Age = 80
				};

				@dbc.Persons.Add(@p1);

				@dbc.SaveChanges();
			}

			Console.WriteLine("アプリケーションを終了します");
			Console.ReadLine();
		}

		static void SqliteInitializeSql()
		{
			string sqltext = "";
			System.Reflection.Assembly assm = System.Reflection.Assembly.GetExecutingAssembly();
			using (var stream = assm.GetManifestResourceStream("EntityFrameworkTest.initialize_sqlite.txt"))
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					sqltext = reader.ReadToEnd();
				}
			}

			using (var @dbc = new MyDbContext())
			{
				@dbc.Database.ExecuteSqlCommand(sqltext);
				@dbc.SaveChanges();
			}
		}
	}

	public class MyDbContext : DbContext
	{
		/// <summary>
		/// データベースに関するグローバル設定を行います。
		/// DbContextを使用する前に、一度だけ呼び出してください。
		/// </summary>
		public static void MyDbContextInitialize()
		{
			// 処理内に、ApplicationContextを参照する箇所があるため、
			// ApplicationContextの初期化が行われた後に呼び出してください。
			Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
			Database.SetInitializer<MyDbContext>(new MyDbContextInitializer());
		}


		public MyDbContext()
			: base("EFTest5_0")
		{

		}

		public DbSet<Person> Persons { get; set; }
	}

	class MyDbContextInitializer : DropCreateDatabaseAlways<MyDbContext>
	{
	}


	[Table("Person")]
	public class Person
	{
		[Key]
		public long PersonId { get; set; }

		public string Name { get; set; }
		public int Age { get; set; }
	}
}
