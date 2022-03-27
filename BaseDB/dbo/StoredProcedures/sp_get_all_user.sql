CREATE PROCEDURE [dbo].[sp_get_all_user]
AS
begin
	select Id, FirstName, LastName
	from dbo.[User];
end
