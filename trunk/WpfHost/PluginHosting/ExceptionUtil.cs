// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace IKriv.PluginHosting
{
    public class ExceptionUtil
    {
        public static string GetUserMessage(Exception ex)
        {
            return String.Join("\r\n",
                GetInnerExceptions(ex).Select(
                    e => String.Format("{0}: {1}", e.GetType().Name, e.Message)).ToArray());
        }

        private static IEnumerable<Exception> GetInnerExceptions(Exception ex)
        {
            for (var exception = ex; exception != null; exception = exception.InnerException)
            {
                yield return exception;
            }
        }

    }
}
