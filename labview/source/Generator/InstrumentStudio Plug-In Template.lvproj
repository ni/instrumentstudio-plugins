<?xml version='1.0' encoding='UTF-8'?>
<Project Type="Project" LVVersion="21008000">
	<Item Name="My Computer" Type="My Computer">
		<Property Name="server.app.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.control.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.tcp.enabled" Type="Bool">false</Property>
		<Property Name="server.tcp.port" Type="Int">0</Property>
		<Property Name="server.tcp.serviceName" Type="Str">My Computer/VI Server</Property>
		<Property Name="server.tcp.serviceName.default" Type="Str">My Computer/VI Server</Property>
		<Property Name="server.vi.callsEnabled" Type="Bool">true</Property>
		<Property Name="server.vi.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="specify.custom.address" Type="Bool">false</Property>
		<Item Name="InstrumentStudio Plug-In Data File" Type="Folder">
			<Item Name="IS Plugin Data File.gplugindata" Type="Document" URL="../_InstrumentStudio Plug-In Template/IS Plugin Data File.gplugindata"/>
		</Item>
		<Item Name="InstrumentStudio Plug-In Main VI" Type="Folder">
			<Item Name="Main.vi" Type="VI" URL="../_InstrumentStudio Plug-In Template/Main.vi"/>
		</Item>
		<Item Name="Dependencies" Type="Dependencies">
			<Item Name="vi.lib" Type="Folder">
				<Item Name="InstrumentStudio Plugin SDK.lvlib" Type="Library" URL="/&lt;vilib&gt;/Plug-In SDKs/InstrumentStudio/InstrumentStudio Plugin SDK.lvlib"/>
			</Item>
		</Item>
		<Item Name="Build Specifications" Type="Build"/>
	</Item>
</Project>
