<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="ServiceBus.Cloud" generation="1" functional="0" release="0" Id="6207e47a-5917-4cea-a292-fd1f1dd18b9d" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="ServiceBus.CloudGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="ServiceBus.WebApi:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/LB:ServiceBus.WebApi:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="ServiceBus.Recive:FlushDbBallsCount" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:FlushDbBallsCount" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.Recive:LogQueueCount" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:LogQueueCount" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.Recive:LogQueueName" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:LogQueueName" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.Recive:LogSqlConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:LogSqlConnectionString" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.Recive:MaxConcurrentCalls" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:MaxConcurrentCalls" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.Recive:Microsoft.ServiceBus.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:Microsoft.ServiceBus.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.Recive:ReceiveBatchCount" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:ReceiveBatchCount" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.Recive:Start" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:Start" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.Recive:TestSpeed" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:TestSpeed" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.Recive:TestSpeedQueueCount" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:TestSpeedQueueCount" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.Recive:ThreadPerQueue" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:ThreadPerQueue" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.Recive:UseTrigger" defaultValue="">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.Recive:UseTrigger" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.ReciveInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.ReciveInstances" />
          </maps>
        </aCS>
        <aCS name="ServiceBus.WebApiInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/MapServiceBus.WebApiInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:ServiceBus.WebApi:Endpoint1">
          <toPorts>
            <inPortMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.WebApi/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapServiceBus.Recive:FlushDbBallsCount" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/FlushDbBallsCount" />
          </setting>
        </map>
        <map name="MapServiceBus.Recive:LogQueueCount" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/LogQueueCount" />
          </setting>
        </map>
        <map name="MapServiceBus.Recive:LogQueueName" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/LogQueueName" />
          </setting>
        </map>
        <map name="MapServiceBus.Recive:LogSqlConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/LogSqlConnectionString" />
          </setting>
        </map>
        <map name="MapServiceBus.Recive:MaxConcurrentCalls" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/MaxConcurrentCalls" />
          </setting>
        </map>
        <map name="MapServiceBus.Recive:Microsoft.ServiceBus.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/Microsoft.ServiceBus.ConnectionString" />
          </setting>
        </map>
        <map name="MapServiceBus.Recive:ReceiveBatchCount" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/ReceiveBatchCount" />
          </setting>
        </map>
        <map name="MapServiceBus.Recive:Start" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/Start" />
          </setting>
        </map>
        <map name="MapServiceBus.Recive:TestSpeed" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/TestSpeed" />
          </setting>
        </map>
        <map name="MapServiceBus.Recive:TestSpeedQueueCount" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/TestSpeedQueueCount" />
          </setting>
        </map>
        <map name="MapServiceBus.Recive:ThreadPerQueue" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/ThreadPerQueue" />
          </setting>
        </map>
        <map name="MapServiceBus.Recive:UseTrigger" kind="Identity">
          <setting>
            <aCSMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.Recive/UseTrigger" />
          </setting>
        </map>
        <map name="MapServiceBus.ReciveInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.ReciveInstances" />
          </setting>
        </map>
        <map name="MapServiceBus.WebApiInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.WebApiInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="ServiceBus.Recive" generation="1" functional="0" release="0" software="D:\资料\办公\Visual Studio 2013\Projects\ServiceBus\ServiceBus.Cloud\csx\Release\roles\ServiceBus.Recive" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="FlushDbBallsCount" defaultValue="" />
              <aCS name="LogQueueCount" defaultValue="" />
              <aCS name="LogQueueName" defaultValue="" />
              <aCS name="LogSqlConnectionString" defaultValue="" />
              <aCS name="MaxConcurrentCalls" defaultValue="" />
              <aCS name="Microsoft.ServiceBus.ConnectionString" defaultValue="" />
              <aCS name="ReceiveBatchCount" defaultValue="" />
              <aCS name="Start" defaultValue="" />
              <aCS name="TestSpeed" defaultValue="" />
              <aCS name="TestSpeedQueueCount" defaultValue="" />
              <aCS name="ThreadPerQueue" defaultValue="" />
              <aCS name="UseTrigger" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;ServiceBus.Recive&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;ServiceBus.Recive&quot; /&gt;&lt;r name=&quot;ServiceBus.WebApi&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.ReciveInstances" />
            <sCSPolicyUpdateDomainMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.ReciveUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.ReciveFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="ServiceBus.WebApi" generation="1" functional="0" release="0" software="D:\资料\办公\Visual Studio 2013\Projects\ServiceBus\ServiceBus.Cloud\csx\Release\roles\ServiceBus.WebApi" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;ServiceBus.WebApi&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;ServiceBus.Recive&quot; /&gt;&lt;r name=&quot;ServiceBus.WebApi&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.WebApiInstances" />
            <sCSPolicyUpdateDomainMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.WebApiUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.WebApiFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="ServiceBus.WebApiUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="ServiceBus.ReciveUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="ServiceBus.ReciveFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="ServiceBus.WebApiFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="ServiceBus.ReciveInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="ServiceBus.WebApiInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="2e831711-356a-4068-ab10-cc2da8545a71" ref="Microsoft.RedDog.Contract\ServiceContract\ServiceBus.CloudContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="4472be91-eb70-498d-95c0-315480a2e83b" ref="Microsoft.RedDog.Contract\Interface\ServiceBus.WebApi:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/ServiceBus.Cloud/ServiceBus.CloudGroup/ServiceBus.WebApi:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>