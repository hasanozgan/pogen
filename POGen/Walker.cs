using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

class DirectoryWalker
{
    public delegate void ProcessDirCallback(DirectoryInfo dir, int level, object obj);
    public delegate void ProcessFileCallback(FileInfo file, int level, object obj);
    
    List<string> FileTypes;
    public DirectoryWalker(ProcessDirCallback dirCallback,
    ProcessFileCallback fileCallback)
    {
        this.dirCallback = dirCallback;
        this.fileCallback = fileCallback;

        FileTypes = new List<string>();
        FileTypes.Add("*.aspx");
        FileTypes.Add("*.ascx");
        FileTypes.Add("*.cs");
        FileTypes.Add("*.master");
    }

    public void Walk(string rootDir, object obj)
    {
        DoWalk(new DirectoryInfo(rootDir), 0, obj);
    }

    void DoWalk(DirectoryInfo dir, int level, object obj)
    {
        foreach (string pattern in FileTypes)
        {
            foreach (FileInfo f in dir.GetFiles(pattern))
            {
                if (fileCallback != null)
                    fileCallback(f, level, obj);
            }
        }
        foreach (DirectoryInfo d in dir.GetDirectories())
        {
            if (dirCallback != null)
                dirCallback(d, level, obj);
            DoWalk(d, level + 1, obj);
        }
    }

    ProcessDirCallback dirCallback;
    ProcessFileCallback fileCallback;
}

public class FilesDateComparer : IComparer<FileInfo>
{
    public int Compare(FileInfo x, FileInfo y)
    {
        int iResult;

        FileInfo oFileX=(FileInfo)x;
        FileInfo oFileY=(FileInfo)y;

        if(oFileX.LastWriteTime==oFileY.LastWriteTime)
        {
            iResult=0;
        }
        else if(oFileX.LastWriteTime<oFileY.LastWriteTime)
        {
            iResult=1;
        }
        else
        {
            iResult=-1;
        }

        return iResult;
    }
}
