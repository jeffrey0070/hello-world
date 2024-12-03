// ReSharper disable EnforceIfStatementBraces
namespace Blue.Application.ReportManager;

using System.Collections.Specialized;
using Blue.Common.Questions;
using Blue.Common.Structure.Project;
using Blue.Common.Structure.Report;

/// <summary>
/// Represents a manager for generating reports.
/// </summary>
public partial class ReportManager
{
    /// <summary>
    /// Gets the response columns for the specified report and project.
    /// </summary>
    /// <param name="report">The report object.</param>
    /// <param name="slaveProject">The slave project, if any.</param>
    /// <param name="cols">
    /// Output parameter that contains the response columns as a comma-separated string.
    /// 
    /// When `multiSubject` and `multiContext` are false, and `slaveProject` is null (master project ID="9c3bae72-8902-45a5-a520-13f4532eb106"):
    /// There is only one response table which is [MasterProjectID].
    /// <c>"[9c3bae72-8902-45a5-a520-13f4532eb106].Q3Row1,[9c3bae72-8902-45a5-a520-13f4532eb106].Q4Row1,[9c3bae72-8902-45a5-a520-13f4532eb106].UserPrincipal,[9c3bae72-8902-45a5-a520-13f4532eb106].SubjectID,[9c3bae72-8902-45a5-a520-13f4532eb106].ConditionID,[9c3bae72-8902-45a5-a520-13f4532eb106].SourceID,[9c3bae72-8902-45a5-a520-13f4532eb106].FilledBy,[9c3bae72-8902-45a5-a520-13f4532eb106].Saved,[9c3bae72-8902-45a5-a520-13f4532eb106].FilloutDate,[9c3bae72-8902-45a5-a520-13f4532eb106].InvitationDate,[9c3bae72-8902-45a5-a520-13f4532eb106].Submitted,[9c3bae72-8902-45a5-a520-13f4532eb106].TaskID,[9c3bae72-8902-45a5-a520-13f4532eb106].AutoKey"</c>.
    ///
    /// When `multiSubject` and `multiContext` are false, and `slaveProject` is not null (master project ID="9c3bae72-8902-45a5-a520-13f4532eb106", slave project ID="6ca3871a-2411-4c31-923a-9184ec1d1892"):
    /// There is only one response table which is [SlaveProjectID].
    /// <c>"[6ca3871a-2411-4c31-923a-9184ec1d1892].Q5Row1 AS Q3Row1,CAST (NULL AS NVARCHAR(MAX)) AS Q4Row1,[6ca3871a-2411-4c31-923a-9184ec1d1892].UserPrincipal,[6ca3871a-2411-4c31-923a-9184ec1d1892].SubjectID,[6ca3871a-2411-4c31-923a-9184ec1d1892].ConditionID,[6ca3871a-2411-4c31-923a-9184ec1d1892].SourceID,[6ca3871a-2411-4c31-923a-9184ec1d1892].FilledBy,[6ca3871a-2411-4c31-923a-9184ec1d1892].Saved,[6ca3871a-2411-4c31-923a-9184ec1d1892].FilloutDate,[6ca3871a-2411-4c31-923a-9184ec1d1892].InvitationDate,[6ca3871a-2411-4c31-923a-9184ec1d1892].Submitted,[6ca3871a-2411-4c31-923a-9184ec1d1892].TaskID,[6ca3871a-2411-4c31-923a-9184ec1d1892].AutoKey"</c>.
    ///
    /// When `multiSubject` or `multiContext`, and `slaveProject` is null (master project ID="3fd631d8-0b5a-4d75-be1a-5b500cc1c90c"):
    /// There are two response tables which are [MasterProjectID] and [MasterProjectIDContextResp]; however, query combinedResp will combine them and produce a single table alias as [MasterProjectID]. So [MasterProjectID] will be used here.
    /// <c>"[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].Q5Row1,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].Q1Row1,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].UserPrincipal,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].SubjectID,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].ConditionID,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].SourceID,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].FilledBy,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].Saved,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].FilloutDate,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].InvitationDate,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].Submitted,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].TaskID,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].AutoKey"</c>.
    ///
    /// When `multiSubject` or `multiContext`, and `slaveProject` is not null (master project ID="3fd631d8-0b5a-4d75-be1a-5b500cc1c90c", slave project ID="0ab7516e-f492-4bc6-9bf4-56fb106e1ab8"):
    /// There are two response tables which are [SlaveProjectID] and [SlaveProjectIDContextResp]; however, query combinedResp will combine them and produce a single table alias as [SlaveProjectID]. So [SlaveProjectID] will be used here.
    /// <c>"[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].Q1Row1 AS Q5Row1,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].Q2Row1 AS Q1Row1,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].UserPrincipal,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].SubjectID,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].ConditionID,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].SourceID,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].FilledBy,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].Saved,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].FilloutDate,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].InvitationDate,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].Submitted,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].TaskID,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].AutoKey"</c>.
    /// </param>
    /// <param name="combCols">
    /// Output parameter that contains the combined response columns for multi-subject or multi-context projects.
    /// 
    /// When `multiSubject` and `multiContext` are false, and `slaveProject` is null (master project ID="9c3bae72-8902-45a5-a520-13f4532eb106"):
    /// <c>""</c>.
    ///
    /// When `multiSubject` and `multiContext` are false, and `slaveProject` is not null (master project ID="9c3bae72-8902-45a5-a520-13f4532eb106", slave project ID="6ca3871a-2411-4c31-923a-9184ec1d1892"):
    /// <c>""</c>.
    ///
    /// When `multiSubject` or `multiContext`, and `slaveProject` is null (master project ID="3fd631d8-0b5a-4d75-be1a-5b500cc1c90c"):
    /// There are two response tables which are [MasterProjectID] and [MasterProjectIDContextResp].
    /// <c>"[3fd631d8-0b5a-4d75-be1a-5b500cc1c90cContextResp].Q5Row1 AS Q5Row1,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].Q1Row1 AS Q1Row1"</c>.
    ///
    /// When `multiSubject` or `multiContext`, and `slaveProject` is not null (master project ID="3fd631d8-0b5a-4d75-be1a-5b500cc1c90c", slave project ID="0ab7516e-f492-4bc6-9bf4-56fb106e1ab8"):
    /// There are two response tables which are [SlaveProjectID] and [SlaveProjectIDContextResp].
    /// <c>"[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8ContextResp].Q1Row1 AS Q5Row1,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].Q2Row1 AS Q1Row1"</c>.
    /// </param>
    /// <param name="colAlias">
    /// Output parameter that contains the column aliases corresponding to the response columns.
    /// 
    /// When `multiSubject` and `multiContext` are false, and `slaveProject` is null (master project ID="9c3bae72-8902-45a5-a520-13f4532eb106"):
    /// <c>"Q3Row1,Q4Row1,UserPrincipal,SubjectID,ConditionID,SourceID,FilledBy,Saved,FilloutDate,InvitationDate,Submitted,TaskID,AutoKey"</c>.
    ///
    /// When `multiSubject` and `multiContext` are false, and `slaveProject` is not null (master project ID="9c3bae72-8902-45a5-a520-13f4532eb106", slave project ID="6ca3871a-2411-4c31-923a-9184ec1d1892"):
    /// <c>"Q3Row1,Q4Row1,UserPrincipal,SubjectID,ConditionID,SourceID,FilledBy,Saved,FilloutDate,InvitationDate,Submitted,TaskID,AutoKey"</c>.
    ///
    /// When `multiSubject` or `multiContext`, and `slaveProject` is null (master project ID="3fd631d8-0b5a-4d75-be1a-5b500cc1c90c"):
    /// <c>"Q5Row1,Q1Row1,UserPrincipal,SubjectID,ConditionID,SourceID,FilledBy,Saved,FilloutDate,InvitationDate,Submitted,TaskID,AutoKey,GroupID,ContextAutoKey,RankingKey,SecondarySubjectFlag"</c>.
    ///
    /// When `multiSubject` or `multiContext`, and `slaveProject` is not null (master project ID="3fd631d8-0b5a-4d75-be1a-5b500cc1c90c", slave project ID="0ab7516e-f492-4bc6-9bf4-56fb106e1ab8"):
    /// <c>"Q5Row1,Q1Row1,UserPrincipal,SubjectID,ConditionID,SourceID,FilledBy,Saved,FilloutDate,InvitationDate,Submitted,TaskID,AutoKey"</c>.
    /// </param>
    /// <remarks>
    /// The behavior of the method depends on the following cases:
    /// 1. When not multiSubject and not multiContext, and slaveProject is null:
    ///    - There is only one response table which is [MasterProjectID].
    ///    cols: <c>"[9c3bae72-8902-45a5-a520-13f4532eb106].Q3Row1,[9c3bae72-8902-45a5-a520-13f4532eb106].Q4Row1,[9c3bae72-8902-45a5-a520-13f4532eb106].UserPrincipal,[9c3bae72-8902-45a5-a520-13f4532eb106].SubjectID,[9c3bae72-8902-45a5-a520-13f4532eb106].ConditionID,[9c3bae72-8902-45a5-a520-13f4532eb106].SourceID,[9c3bae72-8902-45a5-a520-13f4532eb106].FilledBy,[9c3bae72-8902-45a5-a520-13f4532eb106].Saved,[9c3bae72-8902-45a5-a520-13f4532eb106].FilloutDate,[9c3bae72-8902-45a5-a520-13f4532eb106].InvitationDate,[9c3bae72-8902-45a5-a520-13f4532eb106].Submitted,[9c3bae72-8902-45a5-a520-13f4532eb106].TaskID,[9c3bae72-8902-45a5-a520-13f4532eb106].AutoKey"</c>.
    ///    combCols: <c>""</c>.
    ///    colAlias: <c>"Q3Row1,Q4Row1,UserPrincipal,SubjectID,ConditionID,SourceID,FilledBy,Saved,FilloutDate,InvitationDate,Submitted,TaskID,AutoKey"</c>.
    /// 2. When not multiSubject and not multiContext, and slaveProject is not null:
    ///    - There is only one response table which is [SlaveProjectID].
    ///    cols: <c>"[6ca3871a-2411-4c31-923a-9184ec1d1892].Q5Row1 AS Q3Row1,CAST (NULL AS NVARCHAR(MAX)) AS Q4Row1,[6ca3871a-2411-4c31-923a-9184ec1d1892].UserPrincipal,[6ca3871a-2411-4c31-923a-9184ec1d1892].SubjectID,[6ca3871a-2411-4c31-923a-9184ec1d1892].ConditionID,[6ca3871a-2411-4c31-923a-9184ec1d1892].SourceID,[6ca3871a-2411-4c31-923a-9184ec1d1892].FilledBy,[6ca3871a-2411-4c31-923a-9184ec1d1892].Saved,[6ca3871a-2411-4c31-923a-9184ec1d1892].FilloutDate,[6ca3871a-2411-4c31-923a-9184ec1d1892].InvitationDate,[6ca3871a-2411-4c31-923a-9184ec1d1892].Submitted,[6ca3871a-2411-4c31-923a-9184ec1d1892].TaskID,[6ca3871a-2411-4c31-923a-9184ec1d1892].AutoKey"</c>.
    ///    combCols: <c>""</c>.
    ///    colAlias: <c>"Q3Row1,Q4Row1,UserPrincipal,SubjectID,ConditionID,SourceID,FilledBy,Saved,FilloutDate,InvitationDate,Submitted,TaskID,AutoKey"</c>.
    /// 3. When multiSubject or multiContext, and slaveProject is null:
    ///    - There are two response tables which are [MasterProjectID] and [MasterProjectIDContextResp]; however, query combinedResp will combine them and produce a single table alias as [MasterProjectID].
    ///    cols: <c>"[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].Q5Row1,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].Q1Row1,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].UserPrincipal,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].SubjectID,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].ConditionID,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].SourceID,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].FilledBy,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].Saved,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].FilloutDate,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].InvitationDate,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].Submitted,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].TaskID,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].AutoKey"</c>.
    ///    combCols: <c>"[3fd631d8-0b5a-4d75-be1a-5b500cc1c90cContextResp].Q5Row1 AS Q5Row1,[3fd631d8-0b5a-4d75-be1a-5b500cc1c90c].Q1Row1 AS Q1Row1"</c>.
    ///    colAlias: <c>"Q5Row1,Q1Row1,UserPrincipal,SubjectID,ConditionID,SourceID,FilledBy,Saved,FilloutDate,InvitationDate,Submitted,TaskID,AutoKey, GroupID,ContextAutoKey,RankingKey , SecondarySubjectFlag "</c>.
    /// 4. When multiSubject or multiContext, and slaveProject is not null:
    ///    - There are two response tables which are [SlaveProjectID] and [SlaveProjectIDContextResp]; however, query combinedResp will combine them and produce a single table alias as [SlaveProjectID].
    ///    cols: <c>"[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].Q1Row1 AS Q5Row1,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].Q2Row1 AS Q1Row1,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].UserPrincipal,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].SubjectID,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].ConditionID,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].SourceID,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].FilledBy,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].Saved,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].FilloutDate,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].InvitationDate,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].Submitted,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].TaskID,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].AutoKey"</c>.
    ///    combCols: <c>"[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8ContextResp].Q1Row1 AS Q5Row1,[0ab7516e-f492-4bc6-9bf4-56fb106e1ab8].Q2Row1 AS Q1Row1"</c>.
    ///    colAlias: <c>"Q5Row1,Q1Row1,UserPrincipal,SubjectID,ConditionID,SourceID,FilledBy,Saved,FilloutDate,InvitationDate,Submitted,TaskID,AutoKey"</c>.
    /// </remarks>
    public void GetResponseColumns(Report report, Project slaveProject, out string cols, out string combCols, out string colAlias)
    {
        var currentProject = slaveProject ?? report.Project;

        cols = "";
        combCols = "";
        colAlias = "";
        var mappedQid = "";
        var responseTable = currentProject.ResponseTable;
        var contextRespTable = "[" + currentProject.ID.Trim() + "ContextResp]";

        var questionCount = report.Project.BaseProject.Questionnaire.Count;

        var unMappedCol = new StringCollection();

        // Check if response table row size is safe for multisubject or multicontext project [by jlu]
        // If row size exceeds 8K, modify qrating question column data type in response table from Float to SmallInt
        var isRowSizeSafe = true;
        if (report.Project.Definition.IsMultiContext || report.Project.Definition.IsMultiSubject)
        {
            isRowSizeSafe = IS_ResponseTableRecordSize_Safe(currentProject.RealProjectID);
        }
        // [End]

        // [Begin] Report Engine Enhancement
        var usedquestion = GetQuestionnaireFromReportBlocks(report);
        // [End] Report Engine Enhancement

        for (var j = 0; j < questionCount; j++)
        {
            var bq = (BaseQuestion)report.Project.BaseProject.Questionnaire.List[j];

            // [Begin] Report Engine Enhancement
            if (usedquestion != null && !usedquestion.Contains(bq.ID)) continue;
            // [End] Report Engine Enhancement

            BaseQuestion mappedBq = null;
            StringCollection slaveResponseColumns = null;

            if (!(bq is QSectionBox))
            {
                // Skip virtual question for master project
                if (bq.IsVirtualQuestion && slaveProject == null)
                    continue;

                if (slaveProject != null)
                {
                    #region Slave project columns
                    mappedQid = report.PrePostMapping.GetPreQID(slaveProject.ID, bq.ID);
                    if (mappedQid != "")
                    {
                        mappedBq = slaveProject.BaseProject.Questionnaire[mappedQid];
                        if (mappedBq != null)
                        {
                            mappedBq.PopulateSchema();
                            slaveResponseColumns = mappedBq.Fields;
                        }
                        else
                            mappedQid = "";
                    }
                    #endregion
                }

                // [Begin] Check if bq is qrating question and it's not a numeric query. [jlu]
                var changeQRatingColumnType = false;
                var newcolumntype = "smallint";
                // [End]

                #region If it's single selection table, and it has comment or second rating, and row counts are different
                QRatingBox baseSq = null;
                var rowDiffCount = 0;
                var isSingleSelectionTable = false;
                if (bq.TypeName == "Type_Rating" || bq.TypeName == "Type_Rating_Customize")
                {
                    baseSq = (QRatingBox)bq;

                    // [Begin] It's only for multisubject/multicontext projects [jlu]
                    if ((report.Project.Definition.IsMultiContext || report.Project.Definition.IsMultiSubject)
                        && !baseSq.IsNumeric
                        && !isRowSizeSafe)
                    {
                        changeQRatingColumnType = true;
                    }
                    // [End]

                    if (baseSq.hasComments() || baseSq.hasSecondRating())
                    {
                        QRatingBox slaveSq;
                        if (mappedQid != "" && mappedBq != null && (bq.TypeName == "Type_Rating" || bq.TypeName == "Type_Rating_Customize"))
                        {
                            slaveSq = (QRatingBox)mappedBq;
                            rowDiffCount = baseSq.MRows.Count - slaveSq.MRows.Count;
                            isSingleSelectionTable = true;
                        }
                    }
                }
                #endregion

                bq.PopulateSchema();
                var responseColumns = bq.Fields;

                int m;
                for (var n = 0; n < responseColumns.Count; n++)
                {
                    var isEmptyColumn = false;
                    m = n;

                    if (isSingleSelectionTable && rowDiffCount != 0)
                    {
                        #region Calculate the corresponding column index if it's single selection table, and it has comment or second rating, and row counts are different
                        var offset = 2;
                        if (rowDiffCount > 0)
                        {
                            if (baseSq.MRows.Count - rowDiffCount <= n && n < baseSq.MRows.Count)
                                isEmptyColumn = true;

                            if (baseSq.hasComments())
                            {
                                if (baseSq.MRows.Count * offset - rowDiffCount <= n && n < baseSq.MRows.Count * offset)
                                    isEmptyColumn = true;
                                if (baseSq.MRows.Count - 1 < n && n < baseSq.MRows.Count * offset - 1)
                                    m = n - rowDiffCount;
                                offset++;
                            }

                            if (baseSq.hasSecondRating())
                            {
                                if (baseSq.MRows.Count * offset - rowDiffCount <= n && n < baseSq.MRows.Count * offset)
                                    isEmptyColumn = true;

                                if (baseSq.MRows.Count * (offset - 1) - 1 < n && n < baseSq.MRows.Count * offset - 1)
                                    m = n - rowDiffCount * (offset - 1);
                            }
                        }

                        if (rowDiffCount < 0)
                        {
                            // [Begin] There is one bug that when master question rows with comment is less than slave project's
                            // the comment question of last question row can't be matched to slave project because comparing condition is wrong. [jlu]
                            if (baseSq.hasComments() && baseSq.MRows.Count - 1 < n && n <= baseSq.MRows.Count * offset - 1)
                            {
                                m = n - rowDiffCount;
                                offset++;
                            }

                            if (baseSq.hasSecondRating() && baseSq.MRows.Count * (offset - 1) - 1 < n && n <= baseSq.MRows.Count * offset - 1)
                                m = n - rowDiffCount * (offset - 1);
                            // [End]
                        }
                        #endregion
                    }
                    else if ((slaveProject != null && mappedQid == "") || (slaveResponseColumns != null && n >= slaveResponseColumns.Count))
                        isEmptyColumn = true;

                    // Handle virtual question
                    if (bq.IsVirtualQuestion)
                    {
                        if (!isEmptyColumn && unMappedCol.Contains(responseColumns[n].Trim()))
                        {
                            string colQry;
                            // [Begin] [Issue 11055] Generate report with category ranking with merge all project mapping crashes [Jlu]
                            if (bq.TypeName == "Type_Comments_Box" || bq.TypeName == "Type_Comments_Box_Customize"
                                || responseColumns[n].Trim().Contains("Comment")
                                || bq.TypeName == "Type_Comments_Box_CentralizeQP")
                                colQry = "CAST (NULL AS NTEXT) AS " + responseColumns[n].Trim();
                            else
                            {
                                // [Begin] It's only for multisubject/multicontext projects to convert question response column data type [jlu]
                                if (changeQRatingColumnType)
                                {
                                    colQry = "CAST (NULL AS " + newcolumntype + ") AS " + responseColumns[n].Trim();
                                }
                                else
                                {
                                    colQry = "CAST (NULL AS FLOAT) AS " + responseColumns[n].Trim();
                                }
                                // [End]
                            }

                            // [End]
                            var vColQry = "";

                            if (report.Project.Definition.IsMultiSubject || report.Project.Definition.IsMultiContext)
                            {
                                if (mappedBq.IsVirtualQuestion)
                                {
                                    #region Handle mapped virtual question rows that come from different response table
                                    if (((VirtualQRatingBox)mappedBq).MappedQuestionRow != null && ((VirtualQRatingBox)mappedBq).MappedQuestionRow.Count > n)
                                    {
                                        var mappedSlaveQiDs = ((VirtualQRatingBox)mappedBq).MappedQuestionRow[n].Split(',');
                                        if (mappedSlaveQiDs.Length > 2)
                                        {
                                            var realSlaveQuestion = slaveProject.BaseProject.Questionnaire[mappedSlaveQiDs[0]];
                                            if (realSlaveQuestion != null)
                                            {
                                                if (realSlaveQuestion.IsSingleDisplayQuestion)
                                                    vColQry = contextRespTable + "." + slaveResponseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                                else
                                                    vColQry = responseTable + "." + slaveResponseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    if (mappedBq.IsSingleDisplayQuestion)
                                        vColQry = contextRespTable + "." + slaveResponseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                    else
                                        vColQry = responseTable + "." + slaveResponseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                }
                            }
                            else
                                vColQry = responseTable + "." + slaveResponseColumns[n].Trim() + " AS " + responseColumns[n].Trim();

                            if (report.Project.Definition.IsMultiSubject || report.Project.Definition.IsMultiContext)
                                combCols = combCols.Replace(colQry, vColQry);
                            else
                            {
                                cols = cols.Replace(colQry, vColQry);
                                colAlias = responseColumns[n].Trim();
                            }
                        }

                        continue;
                    }

                    #region NonMultiSubject
                    if (slaveProject == null)
                    {
                        #region Master project question columns
                        if (cols.Trim() == "")
                        {
                            cols = responseTable + "." + responseColumns[n].Trim();

                            colAlias = responseColumns[n].Trim();
                        }
                        else
                        {
                            cols += "," + responseTable + "." + responseColumns[n].Trim();

                            colAlias += "," + responseColumns[n].Trim();
                        }
                        #endregion
                    }
                    else
                    {
                        #region Slave project question columns
                        if (cols.Trim() == "")
                        {
                            if (isEmptyColumn)
                            {
                                // [Begin] [Issue 11055] Generate report with category ranking with merge all project mapping crashes [Jlu]
                                if (bq.TypeName == "Type_Comments_Box" || bq.TypeName == "Type_Comments_Box_Customize" ||
                                    responseColumns[n].Trim().Contains("Comment")
                                    || bq.TypeName == "Type_Comments_Box_CentralizeQP")
                                    cols = "CAST (NULL AS NTEXT) AS " + responseColumns[n].Trim();
                                else
                                {
                                    // [Begin] It's only for multisubject/multicontext projects to convert question response column data type [jlu]
                                    if (changeQRatingColumnType)
                                    {
                                        cols = "CAST (NULL AS " + newcolumntype + ") AS " + responseColumns[n].Trim();
                                    }
                                    else
                                    {
                                        cols = "CAST (NULL AS FLOAT) AS " + responseColumns[n].Trim();
                                    }
                                    // [End]
                                }

                                // [End]
                                unMappedCol.Add(responseColumns[n].Trim());
                            }
                            else
                                cols = responseTable + "." + slaveResponseColumns[m].Trim() + " AS " + responseColumns[n].Trim();

                            colAlias = responseColumns[n].Trim();
                        }
                        else
                        {
                            if (isEmptyColumn)
                            {
                                // [Begin] [Issue 11055] Generate report with category ranking with merge all project mapping crashes [Jlu]
                                if (bq.TypeName == "Type_Comments_Box" || bq.TypeName == "Type_Comments_Box_Customize" ||
                                    responseColumns[n].Trim().Contains("Comment")
                                    || bq.TypeName == "Type_Comments_Box_CentralizeQP")
                                    cols += "," + "CAST (NULL AS NTEXT) AS " + responseColumns[n].Trim();
                                else
                                {
                                    // [Begin] It's only for multisubject/multicontext projects to convert question response column data type [jlu]
                                    if (changeQRatingColumnType)
                                    {
                                        cols += "," + "CAST (NULL AS " + newcolumntype + ") AS " + responseColumns[n].Trim();
                                    }
                                    else
                                    {
                                        cols += "," + "CAST (NULL AS FLOAT) AS " + responseColumns[n].Trim();
                                    }
                                    // [End]
                                }

                                // [End]
                                unMappedCol.Add(responseColumns[n].Trim());
                            }
                            else
                                cols += "," + responseTable + "." + slaveResponseColumns[m].Trim() + " AS " + responseColumns[n].Trim();

                            colAlias += "," + responseColumns[n].Trim();
                        }
                        #endregion
                    }
                    #endregion

                    #region For multiSubject
                    if (report.Project.Definition.IsMultiSubject || report.Project.Definition.IsMultiContext)
                    {
                        if (slaveProject == null)
                        {
                            #region Master project question columns
                            if (combCols.Trim() == "")
                            {
                                if (bq.IsSingleDisplayQuestion)
                                {
                                    combCols = contextRespTable + "." + responseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                    // isExistedInContextResp = true;
                                }
                                else
                                {
                                    combCols = responseTable + "." + responseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                    // isExistedInResp = true;
                                }
                            }
                            else
                            {
                                if (bq.IsSingleDisplayQuestion)
                                {
                                    combCols += "," + contextRespTable + "." + responseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                    // isExistedInContextResp = true;
                                }
                                else
                                {
                                    combCols += "," + responseTable + "." + responseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                    // isExistedInResp = true;
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            if (combCols.Trim() == "")
                            {
                                if (isEmptyColumn)
                                {
                                    // [Begin] [Issue 11055] Generate report with category ranking with merge all project mapping crashes [Jlu]
                                    if (bq.TypeName == "Type_Comments_Box" || bq.TypeName == "Type_Comments_Box_Customize" ||
                                        responseColumns[n].Trim().Contains("Comment")
                                        || bq.TypeName == "Type_Comments_Box_CentralizeQP")
                                        combCols = "CAST (NULL AS NTEXT) AS " + responseColumns[n].Trim();
                                    else
                                    {
                                        // [Begin] It's only for multisubject/multicontext projects to convert question response column data type [jlu]
                                        if (changeQRatingColumnType)
                                        {
                                            combCols = "CAST (NULL AS " + newcolumntype + ") AS " + responseColumns[n].Trim();
                                        }
                                        else
                                        {
                                            combCols = "CAST (NULL AS FLOAT) AS " + responseColumns[n].Trim();
                                        }
                                        // [End]
                                    }
                                    // [End]
                                    unMappedCol.Add(responseColumns[n].Trim());
                                }
                                else
                                {
                                    if (mappedBq.IsVirtualQuestion)
                                    {
                                        #region Handle mapped virtual question rows that come from different response table
                                        if (((VirtualQRatingBox)mappedBq).MappedQuestionRow != null && ((VirtualQRatingBox)mappedBq).MappedQuestionRow.Count > n)
                                        {
                                            var mappedSlaveQiDs = ((VirtualQRatingBox)mappedBq).MappedQuestionRow[n].Split(',');
                                            if (mappedSlaveQiDs.Length > 2)
                                            {
                                                var realSlaveQuestion = slaveProject.BaseProject.Questionnaire[mappedSlaveQiDs[0]];
                                                if (realSlaveQuestion != null)
                                                {
                                                    if (realSlaveQuestion.IsSingleDisplayQuestion)
                                                        combCols = contextRespTable + "." + slaveResponseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                                    else
                                                        combCols = responseTable + "." + slaveResponseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        if (mappedBq.IsSingleDisplayQuestion)
                                            combCols = contextRespTable + "." + slaveResponseColumns[m].Trim() + " AS " + responseColumns[n].Trim();
                                        else
                                            combCols = responseTable + "." + slaveResponseColumns[m].Trim() + " AS " + responseColumns[n].Trim();
                                    }
                                }
                            }
                            else
                            {
                                if (isEmptyColumn)
                                {
                                    // [Begin] [Issue 11055] Generate report with category ranking with merge all project mapping crashes [Jlu]
                                    if (bq.TypeName == "Type_Comments_Box" || bq.TypeName == "Type_Comments_Box_Customize" ||
                                        responseColumns[n].Trim().Contains("Comment")
                                        || bq.TypeName == "Type_Comments_Box_CentralizeQP")
                                        combCols += "," + "CAST (NULL AS NTEXT) AS " + responseColumns[n].Trim();
                                    else
                                    {
                                        // [Begin] It's only for multisubject/multicontext projects to convert question response column data type [jlu]
                                        if (changeQRatingColumnType)
                                        {
                                            combCols += "," + "CAST (NULL AS " + newcolumntype + ") AS " + responseColumns[n].Trim();
                                        }
                                        else
                                        {
                                            combCols += "," + "CAST (NULL AS FLOAT) AS " + responseColumns[n].Trim();
                                        }
                                        // [End]
                                    }
                                    // [End]

                                    unMappedCol.Add(responseColumns[n].Trim());
                                }
                                else
                                {
                                    if (mappedBq.IsVirtualQuestion)
                                    {
                                        #region Handle mapped virtual question rows that come from different response table
                                        if (((VirtualQRatingBox)mappedBq).MappedQuestionRow != null && ((VirtualQRatingBox)mappedBq).MappedQuestionRow.Count > n)
                                        {
                                            var mappedSlaveQiDs = ((VirtualQRatingBox)mappedBq).MappedQuestionRow[n].Split(',');
                                            if (mappedSlaveQiDs.Length > 2)
                                            {
                                                var realSlaveQuestion = slaveProject.BaseProject.Questionnaire[mappedSlaveQiDs[0]];
                                                if (realSlaveQuestion != null)
                                                {
                                                    if (realSlaveQuestion.IsSingleDisplayQuestion)
                                                        combCols += "," + contextRespTable + "." + slaveResponseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                                    else
                                                        combCols += "," + responseTable + "." + slaveResponseColumns[n].Trim() + " AS " + responseColumns[n].Trim();
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        if (mappedBq.IsSingleDisplayQuestion)
                                            combCols += "," + contextRespTable + "." + slaveResponseColumns[m].Trim() + " AS " + responseColumns[n].Trim();
                                        else
                                            combCols += "," + responseTable + "." + slaveResponseColumns[m].Trim() + " AS " + responseColumns[n].Trim();
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        // [Begin] Bug 7804:[Individual Report] Column name 'SubjectID' does not exist in the target table or view. jlu
        cols += (string.IsNullOrEmpty(cols) ? "" : ",") + responseTable + ".UserPrincipal" + "," + responseTable + ".SubjectID" + "," + responseTable + ".ConditionID"
                + "," + responseTable + ".SourceID" + "," + responseTable + ".FilledBy" + "," + responseTable + ".Saved" + "," + responseTable + ".FilloutDate"
                + "," + responseTable + ".InvitationDate" + "," + responseTable + ".Submitted" + "," + responseTable + ".TaskID" + "," + responseTable + ".AutoKey";

        colAlias += (string.IsNullOrEmpty(colAlias) ? "" : ",") + "UserPrincipal,SubjectID,ConditionID"
                                                                + ",SourceID,FilledBy,Saved,FilloutDate"
                                                                + ",InvitationDate,Submitted,TaskID,AutoKey";
        // [End] Bug 7804:[Individual Report] Column name 'SubjectID' does not exist in the target table or view. jlu
    }
}
