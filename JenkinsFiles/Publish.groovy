@Library( "X13JenkinsLib" )_

def GetRauVersion()
{
    return readFile( "checkout/dist/version.txt" );
}

pipeline
{
    agent
    {
        label "ubuntu";
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
                copyArtifacts(
                    projectName: "Rau_Build",
                    selector: lastSuccessful()
                );
            }
        }
        stage( "deploy" )
        {
            steps
            {
                withCredentials(
                    [sshUserPrivateKey(
                        credentialsId: "shendrick.net",
                        usernameVariable: "SSHUSER",
                        keyFileVariable: "WEBSITE_KEY" // <- Note: WEBSITE_KEY must be in all quotes below, or rsync won't work if the path has whitespace.
                    )]
                )
                {
                    sh "chmod 700 ./checkout/dist/deploy.sh";
                    sh "./checkout/dist/deploy.sh";
                }
            }
        }
    }
    post
    {
        fixed
        {
            X13SendToTelegramWithCredentials(
                message: "${BUILD_TAG} has been fixed!  See: ${BUILD_URL}",
                botCredsId: "telegram_bot",
                chatCredsId: "telegram_chat_id"
            );
        }
        failure
        {
            X13SendToTelegramWithCredentials(
                message: "${BUILD_TAG} has failed.  See: ${BUILD_URL}",
                botCredsId: "telegram_bot",
                chatCredsId: "telegram_chat_id"
            );
        }
    }
}