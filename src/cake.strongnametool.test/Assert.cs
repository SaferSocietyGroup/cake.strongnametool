using Cake.Core;
using System;
using Xunit;

namespace Cake.StrongNameTool.Test
{
    public partial class Assert
    {
        public static void IsArgumentNullException(Exception exception, string parameterName)
        {
            Xunit.Assert.IsType<ArgumentNullException>(exception);
            Xunit.Assert.Equal(parameterName, ((ArgumentNullException)exception).ParamName);
        }

        public static void IsArgumentException(Exception exception, string parameterName, string message)
        {
            Xunit.Assert.IsType<ArgumentException>(exception);
            Xunit.Assert.Equal(parameterName, ((ArgumentException)exception).ParamName);
            Xunit.Assert.Equal(new ArgumentException(message, parameterName).Message, exception.Message);
        }

        public static void IsCakeException(Exception exception, string message)
        {
            IsExceptionWithMessage<CakeException>(exception, message);
        }

        public static void IsExceptionWithMessage<T>(Exception exception, string message)
            where T : Exception
        {
            Xunit.Assert.IsType<T>(exception);
            Xunit.Assert.Equal(message, exception.Message);
        }
    }
}
