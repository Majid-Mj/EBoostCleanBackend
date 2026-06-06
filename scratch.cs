using System;
public class Program {
    public static void Main() {
        var obj = new { razorpayOrderId = "123" };
        Console.WriteLine(obj.GetType().IsGenericType);
        Console.WriteLine(obj.GetType().GetGenericTypeDefinition());
    }
}
