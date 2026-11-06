namespace Poppolouse_Utilities.fm;

public class FileManager
{
    /// <summary>
    /// Writes a stream to a file.
    /// </summary>
    /// <param name="file">The path to the file.</param>
    /// <param name="stream">The stream to write.</param>
    /// <returns>0 if successful, 1 if the file already exists.</returns>
    /// <exception cref="ArgumentNullException">Thrown when file or stream is null.</exception>
    /// <exception cref="IOException">Thrown when the file already exists.</exception>
    public async Task<int> writeStream(string file, Stream stream)
    {

        // Check for null arguments or if the file already exists
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(stream);
        if (File.Exists(file))
        {
            throw new IOException("File already exists.");
        }
        
        using (FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write))
        {
            stream.CopyTo(fileStream);
        }
        return 0;
    }
    
    
    /// <summary>
    /// Writes a stream to a file in the specified destination directory.
    /// It combines the destination path with the file name to create the full file path.
    /// Provide only the file name in the 'file' parameter, not the full path.
    /// </summary>
    /// <param name="file"> The name of the file to write. </param>
    /// <param name="stream"> The stream to write. </param>
    /// <param name="destination"> The destination directory. </param>
    /// <returns>0 if successful, 1 if the file already exists.</returns>
    /// <exception cref="ArgumentNullException"> Thrown when file, stream, or destination is null. </exception>
    /// <exception cref="IOException"> Thrown when the file already exists. </exception>
    public async Task<int> writeStream(string file, Stream stream, string destination)
    {

        // Check for null arguments or if the file already exists
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(destination);
        
        file = Path.Combine(destination, file);
        if (File.Exists(file))
        {
            throw new IOException("File already exists.");
        }
        
        using (FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write))
        {
            stream.CopyToAsync(fileStream).Wait();
        }
        return 0;
    }
}