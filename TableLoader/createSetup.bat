﻿c:
cd \Visual Studio\Visual Studio TFS\initions-BI\Henry Components\TableLoader\Main\TableLoader
"C:\Program Files (x86)\WiX Toolset v3.7\bin\candle.exe" setup.wxs
"C:\Program Files (x86)\WiX Toolset v3.7\bin\light.exe" -out "Setup TableLoader3 IS2008" -ext WixUIExtension -loc "C:\Program Files (x86)\Windows Installer XML v3\WixUI_de-de.wxl" setup.wixobj
echo Fertig