@Library( "X13JenkinsLib" )_

def CallDevops( String arguments )
{
    dir( "checkout" )
    {
        X13Cmd( "dotnet run --project='./DevOps/DevOps/DevOps.csproj' -- ${arguments}" );
    }
}

pipeline
{
    agent
    {
        label "ubuntu && docker";
    }
    environment
    {
        DOTNET_CLI_TELEMETRY_OPTOUT = 'true'
        DOTNET_NOLOGO = 'true'
    }
    options
    {
        skipDefaultCheckout( true );
    }
    stages
    {
        stage( "Clean" )
        {
            steps
            {
                cleanWs();
            }
        }
        stage( "checkout" )
        {
            steps
            {
                checkout scm;
            }
        }
        stage( "In Dotnet Docker" )
        {
            agent
            {
                docker
                {
                    image 'mcr.microsoft.com/dotnet/sdk:8.0'
                    args "-e HOME='${env.WORKSPACE}'"
                    reuseNode true
                }
            }
            stages
            {
                stage( "Build" )
                {
                    steps
                    {
                        CallDevops( "--target=build" );
                    }
                }
                stage( "Run Tests" )
                {
                    steps
                    {
                        CallDevops( "--target=run_tests" );
                    }
                    post
                    {
                        always
                        {
                            X13ParseMsTestResults(
                                filePattern: "checkout/TestResults/Rau.Tests/*.xml",
                                abortOnFail: true
                            );
                        }
                    }
                }
                stage( "Nuget Pack" )
                {
                    steps
                    {
                        CallDevops( "--target=nuget_pack" );
                    }
                }
                stage( "Publish" )
                {
                    steps
                    {
                        CallDevops( "--target=publish" );
                    }
                }
                stage( "Archive" )
                {
                    steps
                    {
                        archiveArtifacts "checkout/dist/zip/*.*";
                        archiveArtifacts "checkout/dist/version.txt";
                        archiveArtifacts "checkout/dist/nuget/*.*";
                    }
                }
            }
        }
    }
}
