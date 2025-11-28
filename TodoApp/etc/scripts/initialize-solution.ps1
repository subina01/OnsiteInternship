abp install-libs

cd src/TodoApp.DbMigrator && dotnet run && cd -



cd src/TodoApp.Web && dotnet dev-certs https -v -ep openiddict.pfx -p config.auth_server_default_pass_phrase 


exit 0