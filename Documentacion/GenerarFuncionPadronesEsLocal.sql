/**************************
 **** 14-10-2025 11:49 ****
 ****   Versión X      ****
 **************************/

IF EXISTS(SELECT 1 FROM sys.objects WHERE name = N'SP_PADRONES_ESLOCAL') 
   DROP PROCEDURE [dbo].[SP_PADRONES_ESLOCAL]
GO

CREATE  PROCEDURE [dbo].[SP_PADRONES_ESLOCAL] (@codprv char(3))
AS
BEGIN
SET CONCAT_NULL_YIELDS_NULL OFF
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
SET NOCOUNT ON


SELECT DISTINCT d.nrodoc1
FROM VISTACTACTESDOMICILIOS d (nolock) 
WHERE d.cueprefi in ('C', 'P') AND isnull(d.codprv, '')=@codprv



END


GO


