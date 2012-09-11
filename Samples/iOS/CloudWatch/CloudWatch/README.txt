Amazon CloudWatch PutMetricData Sample

This sample demonstrates using the AWS SDK for .NET to sample windows performance counters and use them as custom metrics in CloudWatch.
Prerequisites

You must have a valid Amazon Web Services developer account.
Requires the AWS SDK for .NET. For more information on the AWS SDK for .NET, see http://aws.amazon.com/sdkfornet.
You must be signed up to use Amazon EC2. For more information on Amazon EC2, see http://aws.amazon.com/ec2/.
Running the Sample

To run the Amazon CloudWatch PutMetricData sample:

Open the Amazon_CloudWatch_Sample.sln file in Visual Studio.
Open the App.config file.
Enter your Access Key ID and Secret Access Key:
<add key="AWSAccessKey" value="<Your Access Key ID>"/>   
<add key="AWSSecretKey" value="<Your Secret Access Key>"/>
Save the file.
Run the sample in Debug mode by typing F5.
The sample runs as a console application and updates two CloutWatch custom metrics once every minute, PagingFilePctUsage and PagingFilePctUsagePeak.
It may take 15 minutes for these metrics to appear in the Amazon Cloudwatch console.
Under metrics, in the left hand navigation, select 'EC2' under Metrics.
Select the checkboxes for PagingFilePctUsage and PagingFilePctUsagePeak
Select a time range of 24H, and a period of 5 minutes.
See the Amazon Cloudwatch product page for more inforamation about Amazon CloudWatch.