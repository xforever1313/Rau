//
// Rau - An AT-Proto Bot Framework
// Copyright (C) 2024-2025 Seth Hendrick
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

using Cake.ArgumentBinder;
using Cake.Frosting;
using Seth.CakeLib.TestRunner;

namespace DevOps.RunTests
{
    [TaskName( "run_tests" )]
    [TaskDescription( "Runs all the unit tests.  Pass in --code_coverage=true to run with coverage." )]
    public sealed class RunTestsTask : DevopsTask
    {
        public override void Run( BuildContext context )
        {
            var testConfig = new TestConfig
            {
                ResultsFolder = context.TestResultsFolder,
                TestCsProject = context.TestCsProj
            };

            TestArguments args = context.CreateFromArguments<TestArguments>();

            var runner = new BaseTestRunner( context, testConfig, "Rss2Pds.Tests" );
            if( args.RunWithCodeCoverage )
            {
                runner.RunCodeCoverage( TestArguments.CoverageFilter );
            }
            else
            {
                runner.RunTests();
            }
        }
    }
}
