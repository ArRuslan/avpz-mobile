using UniMobileProject.src.Services.PageServices.Booking;

namespace UniMobileProject.Tests.ServiceTests.BookTests;

public class BookTest
{
    private BookingService service = new BookingService("mytestdb.db");
    [Fact]
    public async void BookingTest() {
        int roomId = 1;
        DateTime checkIn = DateTime.Now;
        DateTime checkOut = DateTime.Now.AddDays(2);

        var response = await service.BookRoom(roomId, checkIn, checkOut);
        Assert.NotNull(response);
    }

}
