# ���������� ����������� ����� .NET SDK ��� ������ ����������
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 5554

# �������� csproj � ��������������� �����������
COPY *.csproj ./
RUN dotnet restore

# �������� ��� ����� � �������� ����������
COPY . ./
RUN dotnet publish -c Release -o out

# ���������� ����������� ����� .NET Runtime ��� ������� ����������
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./
RUN apt-get update
RUN apt-get install -y curl
RUN apt install net-tools

# ��������� ������� ��� ������� ����������
ENTRYPOINT ["dotnet", "HttpBus.dll"]