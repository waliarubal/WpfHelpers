using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NullVoidCreations.WpfHelpers
{
    public class DirectoryWalker: IEnumerable<string>
    {
        string _rootDirectory;
        Func<string, bool> _directoryFilter, _fileFilter;
        bool _recursive;

        #region constructor/destructor

        public DirectoryWalker(string rootDirectory, bool recursive = true): this(rootDirectory, null, null, recursive)
        {

        }

        public DirectoryWalker(string rootDirectory, Func<string, bool> fileFilter, bool recursive = true): this(rootDirectory, null, fileFilter, recursive)
        {

        }

        public DirectoryWalker(string rootDirectory, Func<string, bool> directoryFilter, Func<string, bool> fileFilter, bool recursive)
        {
            if (string.IsNullOrEmpty(rootDirectory))
                throw new ArgumentNullException("rootDirectory");

            _rootDirectory = rootDirectory;
            _directoryFilter = directoryFilter;
            _fileFilter = fileFilter;
            _recursive = recursive;
        }

        #endregion

        public IEnumerator<string> GetEnumerator()
        {
            var directories = new Queue<string>();
            var files = new Queue<string>();

            directories.Enqueue(_rootDirectory);
            while (files.Count > 0 || directories.Count > 0)
            {
                if (files.Count > 0)
                    yield return files.Dequeue();

                if (directories.Count > 0)
                {
                    var directory = directories.Dequeue();

                    if (_recursive)
                    {
                        string[] subDirectories = null;
                        try
                        {
                            subDirectories = Directory.GetDirectories(directory);
                        }
                        catch
                        {

                        }

                        if (subDirectories != null)
                        {
                            foreach (var path in subDirectories)
                            {
                                if (_directoryFilter == null || _directoryFilter(path))
                                    directories.Enqueue(path);
                            }
                        }
                    }

                    string[] subFiles = null;
                    try
                    {
                        subFiles = Directory.GetFiles(directory);
                    }
                    catch
                    {

                    }

                    if (subFiles != null)
                    {
                        foreach (var path in subFiles)
                        {
                            if (_fileFilter == null || _fileFilter(path))
                                files.Enqueue(path);
                        }
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
