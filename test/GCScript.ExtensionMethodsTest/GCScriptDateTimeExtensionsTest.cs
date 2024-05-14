using GCScript.ExtensionMethods;

namespace GCScript.ExtensionMethodsTest;

public class GCScriptDateTimeExtensionsTest
{
    [Fact(DisplayName = "ToISO8601 with DateTime should return ISO8601 string")]
    public void ToISO8601_WithDateTime_ShouldReturnISO8601String()
    {
        // Arrange
        var input1 = new DateTime(1998, 5, 19, 9, 50, 29);
        var input2 = new DateTime?(input1);
        var input3 = new DateTimeOffset(input1);
        var input4 = new DateTimeOffset?(input3);

        var expected = "1998-05-19T09:50:29Z";

        // Act
        var result1 = input1.ToISO8601();
        var result2 = input2.ToISO8601();
        var result3 = input3.ToISO8601();
        var result4 = input4.ToISO8601();

        // Assert
        Assert.Equal(expected, result1);
        Assert.Equal(expected, result2);
        Assert.Equal(expected, result3);
        Assert.Equal(expected, result4);
    }

    [Fact(DisplayName = "ToDDMMYYYY with DateTime should return dd-MM-yyyy string")]
    public void ToDDMMYYYY_WithDateTime_ShouldReturnDDMMYYYYString()
    {
        // Arrange
        var input1 = new DateTime(1998, 5, 19, 9, 50, 29);
        var input2 = new DateTime?(input1);
        var input3 = new DateTimeOffset(input1);
        var input4 = new DateTimeOffset?(input3);

        var expected1 = "19-05-1998";
        var expected2 = "19/05/1998";

        // Act
        var result1 = input1.ToDDMMYYYY();
        var result2 = input2.ToDDMMYYYY();
        var result3 = input3.ToDDMMYYYY();
        var result4 = input4.ToDDMMYYYY();
        var result5 = input1.ToDDMMYYYY('/');
        var result6 = input2.ToDDMMYYYY('/');
        var result7 = input3.ToDDMMYYYY('/');
        var result8 = input4.ToDDMMYYYY('/');

        // Assert
        Assert.Equal(expected1, result1);
        Assert.Equal(expected1, result2);
        Assert.Equal(expected1, result3);
        Assert.Equal(expected1, result4);
        Assert.Equal(expected2, result5);
        Assert.Equal(expected2, result6);
        Assert.Equal(expected2, result7);
        Assert.Equal(expected2, result8);
    }

    [Fact(DisplayName = "ToYYYYMMDD with DateTime should return yyyy-MM-dd string")]
    public void ToYYYYMMDD_WithDateTime_ShouldReturnYYYYMMDDString()
    {
        // Arrange
        var input1 = new DateTime(1998, 5, 19, 9, 50, 29);
        var input2 = new DateTime?(input1);
        var input3 = new DateTimeOffset(input1);
        var input4 = new DateTimeOffset?(input3);

        var expected1 = "1998-05-19";
        var expected2 = "1998/05/19";

        // Act
        var result1 = input1.ToYYYYMMDD();
        var result2 = input2.ToYYYYMMDD();
        var result3 = input3.ToYYYYMMDD();
        var result4 = input4.ToYYYYMMDD();
        var result5 = input1.ToYYYYMMDD('/');
        var result6 = input2.ToYYYYMMDD('/');
        var result7 = input3.ToYYYYMMDD('/');
        var result8 = input4.ToYYYYMMDD('/');

        // Assert
        Assert.Equal(expected1, result1);
        Assert.Equal(expected1, result2);
        Assert.Equal(expected1, result3);
        Assert.Equal(expected1, result4);
        Assert.Equal(expected2, result5);
        Assert.Equal(expected2, result6);
        Assert.Equal(expected2, result7);
        Assert.Equal(expected2, result8);
    }

    [Fact(DisplayName = "ToMMDDYYYY with DateTime should return MM-dd-yyyy string")]
    public void ToMMDDYYYY_WithDateTime_ShouldReturnMMDDYYYYString()
    {
        // Arrange
        var input1 = new DateTime(1998, 5, 19, 9, 50, 29);
        var input2 = new DateTime?(input1);
        var input3 = new DateTimeOffset(input1);
        var input4 = new DateTimeOffset?(input3);

        var expected1 = "05-19-1998";
        var expected2 = "05/19/1998";

        // Act
        var result1 = input1.ToMMDDYYYY();
        var result2 = input2.ToMMDDYYYY();
        var result3 = input3.ToMMDDYYYY();
        var result4 = input4.ToMMDDYYYY();
        var result5 = input1.ToMMDDYYYY('/');
        var result6 = input2.ToMMDDYYYY('/');
        var result7 = input3.ToMMDDYYYY('/');
        var result8 = input4.ToMMDDYYYY('/');

        // Assert
        Assert.Equal(expected1, result1);
        Assert.Equal(expected1, result2);
        Assert.Equal(expected1, result3);
        Assert.Equal(expected1, result4);
        Assert.Equal(expected2, result5);
        Assert.Equal(expected2, result6);
        Assert.Equal(expected2, result7);
        Assert.Equal(expected2, result8);
    }

    [Fact(DisplayName = "ToHHMM with DateTime should return HH:mm string")]
    public void ToHHMM_WithDateTime_ShouldReturnHHMMString()
    {
        // Arrange
        var input1 = new DateTime(1998, 5, 19, 9, 50, 29);
        var input2 = new DateTime?(input1);
        var input3 = new DateTimeOffset(input1);
        var input4 = new DateTimeOffset?(input3);

        var expected1 = "09:50";
        var expected2 = "09-50";

        // Act
        var result1 = input1.ToHHMM();
        var result2 = input2.ToHHMM();
        var result3 = input3.ToHHMM();
        var result4 = input4.ToHHMM();
        var result5 = input1.ToHHMM('-');
        var result6 = input2.ToHHMM('-');
        var result7 = input3.ToHHMM('-');
        var result8 = input4.ToHHMM('-');

        // Assert
        Assert.Equal(expected1, result1);
        Assert.Equal(expected1, result2);
        Assert.Equal(expected1, result3);
        Assert.Equal(expected1, result4);
        Assert.Equal(expected2, result5);
        Assert.Equal(expected2, result6);
        Assert.Equal(expected2, result7);
        Assert.Equal(expected2, result8);
    }

    [Fact(DisplayName = "IsToday with DateTime should return true if date is today")]
    public void IsToday_WithDateTime_ShouldReturnTrueIfDateIsToday()
    {
        // Arrange
        var input1 = DateTime.Today;
        var input2 = DateTime.Today.AddDays(-1);
        var input3 = DateTime.Today.AddDays(1);
        var input4 = new DateTime? (input1);
        var input5 = new DateTime? (input2);
        var input6 = new DateTime? (input3);
        var input7 = new DateTimeOffset(input1);
        var input8 = new DateTimeOffset(input2);
        var input9 = new DateTimeOffset(input3);
        var input10 = new DateTimeOffset? (input1);
        var input11 = new DateTimeOffset? (input2);
        var input12 = new DateTimeOffset? (input3);

        // Act
        var result1 = input1.IsToday();
        var result2 = input2.IsToday();
        var result3 = input3.IsToday();
        var result4 = input4.IsToday();
        var result5 = input5.IsToday();
        var result6 = input6.IsToday();
        var result7 = input7.IsToday();
        var result8 = input8.IsToday();
        var result9 = input9.IsToday();
        var result10 = input10.IsToday();
        var result11 = input11.IsToday();
        var result12 = input12.IsToday();

        // Assert
        Assert.True(result1);
        Assert.False(result2);
        Assert.False(result3);
        Assert.True(result4);
        Assert.False(result5);
        Assert.False(result6);
        Assert.True(result7);
        Assert.False(result8);
        Assert.False(result9);
        Assert.True(result10);
        Assert.False(result11);
        Assert.False(result12);
    }
}
