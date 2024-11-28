﻿//
// Rss2Pds - A bot that reads RSS feeds and posts them to a AT-Proto PDS node
// Copyright (C) 2024 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using Cake.Frosting;
using Seth.CakeLib.TestRunner;

namespace DevOps.RunTests
{
    [TaskName( "run_tests" )]
    public sealed class RunTestsTask : DevopsTask
    {
        public override void Run( BuildContext context )
        {
            var testConfig = new TestConfig
            {
                ResultsFolder = context.TestResultsFolder,
                TestCsProject = context.TestCsProj
            };

            var runner = new BaseTestRunner( context, testConfig, "Rss2Pds.Tests" );
            runner.RunTests();
        }
    }
}
