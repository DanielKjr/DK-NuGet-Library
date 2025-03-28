using DK.GenericLibrary.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepoUnitTests.Fakes;
using static NUnit.Framework.Assert;


namespace RepoUnitTests
{
	public class Tests 
	{
		private ServiceProvider _serviceProvider;
	
		[SetUp]
		public void Setup()
		{
			_serviceProvider = TestServiceProvider.GetServiceProvider();
		}


		[TearDown]
		public async Task Dispose()
		{
			await _serviceProvider.DisposeAsync();
		}


		[Test]
		public async Task CorrectlyCreatesContext()
		{
			using var scope = _serviceProvider.CreateScope();
			var dbContextFactory = _serviceProvider.GetService<IDbContextFactory<FakeContext>>();
			var context = await dbContextFactory?.CreateDbContextAsync()!;
			That(context.BasicClasses != null);
		}

		[Test]
		public async Task CanAdd()
		{
			//Arrange
			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			var baseClass = new BasicClass { TestField = "Test1", Refnr = 1 };
			baseClass.BasicEntries.Add(new BasicEntry() { BasicClassId = baseClass.Id, ValueToLoad = "Test1" });

			// Act
			await repository.AddItem(baseClass);

			//Assert
			var result = await repository.GetItem<BasicClass>(q => q.Where(x => x.Id == baseClass.Id));
			That(result != null);
			
		}

		[Test]
		public async Task CanAddMultiple()
		{

			List<BasicClass> basicClassList = new List<BasicClass>()
			{
				new BasicClass { TestField = "Test1", Refnr = 1 },
				new BasicClass { TestField = "Test2", Refnr = 2 },
				new BasicClass { TestField = "Test3", Refnr = 3 }
			};


			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();

			await repository.AddItems(basicClassList);

			var result = await repository.GetAllItems<BasicClass>();
			That(result.Count == 3);
			
		}

		[Test]
		public async Task CanRemove()
		{

			//Arrange
			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			var baseClass = new BasicClass { TestField = "Test1", Refnr = 1 };
			baseClass.BasicEntries.Add(new BasicEntry() { BasicClassId = baseClass.Id, ValueToLoad = "Test1" });
			await repository.AddItem(baseClass);

			//Ensure it is added
			var addedItem = await repository.GetItem<BasicClass>(q => q.Where(x => x.Id == baseClass.Id));
			That(addedItem != null);
			//Act
			await repository.RemoveItem(baseClass);

			//Assert
			var result = await repository.GetItem<BasicClass>(q => q.Where(x => x.Id == baseClass.Id));
			That(result == null);

		}


		[Test]
		public async Task CanRemoveByExpression()
		{

			//Arrange
			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			var baseClass = new BasicClass { TestField = "Test1", Refnr = 1 };
			baseClass.BasicEntries.Add(new BasicEntry() { BasicClassId = baseClass.Id, ValueToLoad = "Test1" });

			await repository.AddItem(baseClass);
			
			var addedItem = await repository.GetItem<BasicClass>(q => q.Where(x => x.Id == baseClass.Id));
			That(addedItem != null);

			await repository.RemoveItem<BasicClass>(x => x.Id == baseClass.Id);

			var result = await repository.GetAllItems<BasicClass>();
			Is.EqualTo(result.Count == 0);


		}


		[Test]
		public async Task CanRemoveMultiple()
		{
			List<BasicClass> basicClassList = new List<BasicClass>()
			{
				new BasicClass { TestField = "Test1", Refnr = 1 },
				new BasicClass { TestField = "Test2", Refnr = 2 },
				new BasicClass { TestField = "Test3", Refnr = 3 }
			};


			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();

			await repository.AddItems(basicClassList);

			await repository.RemoveItems<BasicClass>(basicClassList);

			var result = await repository.GetAllItems<BasicClass>();
			That(result.Count == 0);
		}

		[Test]
		public async Task CanRemoveMultipleByExpression()
		{
			List<BasicClass> basicClassList = new List<BasicClass>()
			{
				new BasicClass { TestField = "Test1", Refnr = 1 },
				new BasicClass { TestField = "Test2", Refnr = 2 },
				new BasicClass { TestField = "Test3", Refnr = 3 }
			};


			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();

			await repository.AddItems(basicClassList);
			await repository.RemoveItems<BasicClass>(x => x.Where(y => y.Refnr == 1));

			var result = await repository.GetAllItems<BasicClass>();
			That(result.Count == 2);
		}

		[Test]
		public async Task CanGetByExpression()
		{
			//Arrange
			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			var baseClass = new BasicClass { TestField = "Test1", Refnr = 1 };
			baseClass.BasicEntries.Add(new BasicEntry() { BasicClassId = baseClass.Id, ValueToLoad = "Test1" });

			// Act
			await repository.AddItem(baseClass);

			//Assert
			var result = await repository.GetItem<BasicClass>(q => q.Where(x => x.Id == baseClass.Id));
			That(result?.TestField, Is.EqualTo("Test1"));
				
		}


		[Test]
		public async Task CanGetByExpressionWithCollection()
		{
		
			//Create instance that is disposed of at end of function
			//Also makes use of a specific dependency injection methodology
			using var scope = _serviceProvider.CreateScope();
			//use reference to create an in memory database through interface and generics
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();
			//Create instance of class used for testing
			var baseClass = new BasicClass { TestField = "Test1", Refnr = 1, BasicEntries = new List<BasicEntry>(){ new BasicEntry() { ValueToLoad = "Test1" } } };

			//Add item to database
			await repository.AddItem(baseClass);

			//Get item from database with Func expressions implementing Queryable interface
			var result = await repository.GetItem<BasicClass>(q => q.Where(x => x.Id == baseClass.Id).Include(x => x.BasicEntries));
			//Assert that the returned instance is the same as what we added
			That(result?.TestField, Is.EqualTo("Test1"));
			That(result?.BasicEntries.First().Id == baseClass.BasicEntries.First().Id);


		}


		[Test]
		public async Task CanGetAllWithoutExpression()
		{
		
			var baseClass = new BasicClass { TestField = "Test1", Refnr = 1 };
			var baseClass2 = new BasicClass { TestField = "Test2", Refnr = 2 };
			baseClass.BasicEntries.Add(new BasicEntry() { BasicClassId = baseClass.Id, ValueToLoad = "Test1" });
			baseClass.BasicEntries.Add(new BasicEntry() { BasicClassId = baseClass2.Id, ValueToLoad = "Test2" });

			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();

			await repository.AddItem(baseClass);
			await repository.AddItem(baseClass2);

			var result = await repository.GetAllItems<BasicClass>();
			That(result.Count == 2);
		
		}

		[Test]
		public async Task CanGetAllWithExpression()
		{

			var baseClass = new BasicClass { TestField = "Test1", Refnr = 1 };
			var baseClass2 = new BasicClass { TestField = "Test2", Refnr = 2 };
			baseClass.BasicEntries.Add(new BasicEntry() { BasicClassId = baseClass.Id, ValueToLoad = "Test1" });
			baseClass.BasicEntries.Add(new BasicEntry() { BasicClassId = baseClass2.Id, ValueToLoad = "Test2" });

			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();

			await repository.AddItem(baseClass);
			await repository.AddItem(baseClass2);

			var result = await repository.GetAllItems<BasicClass>(q=> q.Where(x => x.Id == baseClass.Id));
			That(result.Count == 1);
			

		}

		[Test]
		public async Task CanGetAllForColumn()
		{
			List<BasicClass> basicClassList = new List<BasicClass>()
			{
				new BasicClass { TestField = "Test1", Refnr = 1 },
				new BasicClass { TestField = "Test2", Refnr = 2 },
				new BasicClass { TestField = "Test3", Refnr = 3 }
			};


			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();

			await repository.AddItems(basicClassList);

			List<string> result = await repository.GetAllForColumn<BasicClass, string>(x => x.Select(s => s.TestField)!);
			List<int> test2Result = await repository.GetAllForColumnStruct<BasicClass, int>(x => x.Select(s => s.Refnr));
			That(result.Count == 3);
			That(test2Result.Count == 3 && test2Result.First() == 1);
		}


		[Test]
		public async Task CanUpdateItem()
		{
			List<BasicClass> basicClassList = new List<BasicClass>()
			{
				new BasicClass { TestField = "Test1", Refnr = 1 },
				new BasicClass { TestField = "Test2", Refnr = 2 },
				new BasicClass { TestField = "Test3", Refnr = 3 }
			};


			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();

			await repository.AddItems(basicClassList);

			var itemToEdit = repository.GetItem<BasicClass>(x => x.Where(q => q.Id == basicClassList.First().Id));
			itemToEdit.Result.TestField = "Edited";
			await repository.UpdateItem(itemToEdit.Result);

			var result = await repository.GetItem<BasicClass>(x => x.Where(q => q.Id == basicClassList.First().Id));
			That(result.TestField == "Edited");
		}


		[Test]
		public async Task CanUpdateMultipleItems()
		{
			List<BasicClass> basicClassList = new List<BasicClass>()
			{
				new BasicClass { TestField = "Test1", Refnr = 1 },
				new BasicClass { TestField = "Test2", Refnr = 2 },
				new BasicClass { TestField = "Test3", Refnr = 3 }
			};


			using var scope = _serviceProvider.CreateScope();
			var repository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<FakeContext>>();

			await repository.AddItems(basicClassList);

			var items = repository.GetAllItems<BasicClass>();
			foreach (var item in items.Result)
			{
				item.TestField = "Edited";
			}

			await repository.UpdateItems<BasicClass>(await items);

			var result = await repository.GetAllItems<BasicClass>();
			That(result.TrueForAll(x => x.TestField == "Edited"));
		}
	}
}