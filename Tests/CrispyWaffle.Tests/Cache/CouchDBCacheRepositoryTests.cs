using System;
using System.Threading;
using CrispyWaffle.Configuration;
using CrispyWaffle.CouchDB;
using CrispyWaffle.CouchDB.DTOs;
using Xunit;

namespace CrispyWaffle.Tests.Cache;

[Collection("Sequential")]
public class CouchDBCacheRepositoryTests : IDisposable
{
    private readonly CouchDBCacheRepository _repo;
    public CouchDBCacheRepositoryTests()
    {
        var conn = new Connection();
        conn.Host = "http://localhost";
        conn.Port = 5984;
        conn.Credentials.Username = "Admin";
        conn.Credentials.Password = "myP@ssw0rd";

        _repo = new CouchDBCacheRepository(conn, AuthType.Basic);
    }
    /// <summary>
    /// Tests the Get and Set functionality of the CouchDoc repository.
    /// </summary>
    /// <remarks>
    /// This test method creates a new instance of <see cref="CouchDoc"/> and sets it in the repository using a unique identifier generated by <see cref="Guid.NewGuid"/>.
    /// It then retrieves the document from the repository and asserts that the key of the retrieved document matches the original document's key.
    /// Finally, it cleans up by removing the document from the repository.
    /// This ensures that the repository's Get and Set methods work correctly and that the document can be successfully stored and retrieved.
    /// </remarks>
    [Fact]
    public void GetAndSetCouchDocTest()
    {
        var doc = new CouchDoc();

        _repo.Set(doc, Guid.NewGuid().ToString());

        var docDB = _repo.Get<CouchDoc>(doc.Key);

        Assert.True(doc.Key == docDB.Key);

        _repo.Remove(doc.Key);
    }
    /// <summary>
    /// Tests the Get and Set functionality for specific Car objects in the repository.
    /// </summary>
    /// <remarks>
    /// This unit test verifies that specific instances of the Car class can be correctly set and retrieved from the repository.
    /// It creates two Car objects with different makers, sets them in the repository with unique keys and subkeys, 
    /// and then retrieves them to ensure that the properties match the expected values. 
    /// The test also checks that the objects can be removed from the repository after verification.
    /// The assertions confirm that the keys and makers of the retrieved Car objects are as expected.
    /// </remarks>
    /// <exception cref="System.Exception">
    /// This method may throw an exception if the repository operations fail, such as when trying to retrieve or remove a non-existent item.
    /// </exception>
    [Fact]
    public void GetAndSetSpecificTest()
    {
        var docOne = new Car("MakerOne");

        _repo.SetSpecific(docOne, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        var docTwo = new Car("MakerTwo");

        _repo.SetSpecific(docTwo, Guid.NewGuid().ToString());

        var docDB = _repo.GetSpecific<Car>(docOne.Key);

        Assert.True(docOne.Key == docDB.Key
            && docOne.SubKey == docDB.SubKey
            && docOne.Maker == "MakerOne");

        docDB = _repo.GetSpecific<Car>(docTwo.Key);

        Assert.True(docTwo.Key == docDB.Key
            && docTwo.Maker == "MakerTwo");

        _repo.RemoveSpecific<Car>(docOne.Key);
        _repo.RemoveSpecific<Car>(docTwo.Key);
    }
    /// <summary>
    /// Tests the removal of a CouchDoc from the repository.
    /// </summary>
    /// <remarks>
    /// This test method creates a new instance of <see cref="CouchDoc"/>, sets it in the repository with a unique key, 
    /// and then removes it from the repository. After the removal, it attempts to retrieve the document using its key. 
    /// The assertion checks that the retrieved document is the default value, indicating that the document has been successfully removed.
    /// This method is decorated with the [Fact] attribute, indicating that it is a unit test that should be executed by the test runner.
    /// </remarks>
    [Fact]
    public void RemoveCouchDocTest()
    {
        var doc = new CouchDoc();

        _repo.Set(doc, Guid.NewGuid().ToString());

        _repo.Remove(doc.Key);

        var docDB = _repo.Get<CouchDoc>(doc.Key);

        Assert.True(docDB == default);
    }
    /// <summary>
    /// Tests the removal of a specific Car document from the repository.
    /// </summary>
    /// <remarks>
    /// This test method creates a new instance of a Car object with a specified maker. 
    /// It then sets this object in the repository with a unique identifier. 
    /// After that, it calls the method to remove the specific Car document using its key. 
    /// Finally, it retrieves the document from the repository to assert that it has been successfully removed.
    /// The assertion checks that the retrieved document is the default value, indicating that the document no longer exists in the repository.
    /// </remarks>
    [Fact]
    public void RemoveSpecificTest()
    {
        var doc = new Car("Maker");

        _repo.SetSpecific(doc, Guid.NewGuid().ToString());

        _repo.RemoveSpecific<Car>(doc.Key);

        var docDB = _repo.Get<CouchDoc>(doc.Key);

        Assert.True(docDB == default);
    }
    /// <summary>
    /// Tests the functionality of clearing the database repository.
    /// </summary>
    /// <remarks>
    /// This test method first populates the repository with several instances of <see cref="CouchDoc"/> by calling the <see cref="_repo.Set"/> method multiple times.
    /// After adding the documents, it invokes the <see cref="_repo.Clear"/> method to remove all entries from the repository.
    /// Finally, it checks that the document count in the repository is zero by calling <see cref="_repo.GetDocCount{CouchDoc}"/>.
    /// The assertion ensures that the clear operation was successful and that no documents remain in the repository.
    /// </remarks>
    [Fact]
    public void DatabaseClearTest()
    {
        _repo.Set(new CouchDoc(), Guid.NewGuid().ToString());
        _repo.Set(new CouchDoc(), Guid.NewGuid().ToString());
        _repo.Set(new CouchDoc(), Guid.NewGuid().ToString());
        _repo.Set(new CouchDoc(), Guid.NewGuid().ToString());

        _repo.Clear();

        var count = _repo.GetDocCount<CouchDoc>();

        Assert.True(count == 0);
    }
    /// <summary>
    /// Tests the TTL (Time-To-Live) functionality of the CouchDB repository.
    /// </summary>
    /// <remarks>
    /// This test verifies that a document can be stored in the CouchDB repository with a specified TTL.
    /// It creates a new instance of <see cref="CouchDoc"/>, assigns it a unique key, and stores it in the repository with a TTL of 5 seconds.
    /// After storing, it retrieves the document and asserts that the retrieved document's key matches the original document's key.
    /// The test then waits for 6 seconds to ensure that the document has expired, and attempts to retrieve it again.
    /// Finally, it asserts that the retrieved document is null, confirming that the TTL functionality is working as expected.
    /// </remarks>
    /// <exception cref="System.Exception">Thrown when the assertions fail, indicating that the expected behavior did not occur.</exception>
    [Fact]
    public void TTLGetTest()
    {
        var doc = new CouchDoc()
        {
            Key = Guid.NewGuid().ToString()
        };

        _repo.Set(new CouchDoc(), doc.Key, new TimeSpan(0, 0, 5));
        var fromDB = _repo.Get<CouchDoc>(doc.Key);

        Assert.True(doc.Key == fromDB.Key);

        Thread.Sleep(6000);

        fromDB = _repo.Get<CouchDoc>(doc.Key);

        Assert.True(fromDB == null);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the resources used by the current instance of the class.
    /// </summary>
    /// <param name="disposing">A boolean value indicating whether the method was called directly or by the garbage collector.</param>
    /// <remarks>
    /// This method is part of the IDisposable pattern. When <paramref name="disposing"/> is true, it indicates that the method has been called directly 
    /// or through the Dispose method, and managed resources should be disposed of. If <paramref name="disposing"/> is false, it indicates that the method 
    /// has been called by the finalizer and only unmanaged resources should be released. In this implementation, if disposing is true, it calls 
    /// the Dispose method on the repository (_repo) if it is not null, ensuring that any resources held by it are properly released.
    /// </remarks>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _repo?.Dispose();
        }
    }
}

public class Car : CouchDoc
{
    public Car(string maker)
    {
        Maker = maker;
    }

    public string Maker { get; set; }
}
