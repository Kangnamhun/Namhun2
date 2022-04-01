echo off
del/q *.bak
del data\*.*/q
cls
path="D:\devtool\jdk1.8\bin";
set classpath = .;"D:\devtool\jdk1.8\lib\tools.jar";"D:\devtool\jdk1.8\lib\dt.jar";
javac ItemTool.java
java ItemTool


@echo ======================================
@echo 		Client File Move
@echo ======================================
copy iteminfo.xml		..\Assets\Resources\txt\iteminfo.xml

copy gameinfo.xml		..\Assets\Resources\txt\gameinfo.xml

copy tooltip.xml		..\Assets\Resources\txt\tooltip.xml




pause
