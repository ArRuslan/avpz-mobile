using UniMobileProject.src.Services.PageServices.Booking;

namespace UniMobileProject.Tests.ServiceTests.BookTests;

public class BookTest
{
    private BookingService service = new BookingService("mytestdb.db");
    [Fact]
    public async void BookingTest() {
        int roomId = 2;
        DateTime checkIn = DateTime.Now.AddDays(3);
        DateTime checkOut = DateTime.Now.AddDays(5);

        var response = await service.BookRoom(roomId, checkIn, checkOut);
        Assert.NotNull(response);
    }

}
