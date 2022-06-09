using System;
using System.IO;

interface Shape
{
    public float area();
    public float permimeter();
}

class Circle : Shape
{
    private float radius;

    public float area()
    {
        return (float)(Math.PI * Math.Pow(radius, 2));
    }

    public float permimeter()
    {
        return (float)(2.0f * Math.PI * radius);
    }
    
    public void setUp(float r)
    {
        radius = r;
    }
}

class Ellipse : Shape
{
    private float r1;
    private float r2;
    public float area()
    {
        return (float)(Math.PI * r1 * r2);
    }

    public float permimeter()
    {
        // Formula is approximation from Ramanujan's Ellipses Perimeter Formula.
        return (float)(Math.PI * (3 * (r1 + r2) - Math.Sqrt(((3 * r1) + r2) * (r1 + (3 * r2)))));
    }
    
    public void setUp(float rad1, float rad2)
    {
        r1 = rad1;
        r2 = rad2;
    }
}

class Polygon : Shape
{
    // use shoelace to get the area with dots on the points.
    private float[] xcoord = null!;
    private float[] ycoord = null!;
    private int numpoints = 0; // used to loop points

    public void SetSize(int pts)
    {
        numpoints = (pts-2)/4;
    }
    public float area()
    {
        float a = 0f;
        for (int i = 0; i < numpoints; i++)
        {
            if (i + 1 == numpoints)
            {
                // We have reached the last point and need to connect it to the beginning
                // a += xcoord[i] * ycoord[0] - ycoord[i] * xcoord[0];
                break;
            }
            else
            {
                a += xcoord[i] * ycoord[i + 1] - ycoord[i] * xcoord[i + 1];
            }
        }

        return a;
    }

    public float permimeter()
    {
        float perry = 0f;
        
        for (int i = 0; i < numpoints; i++)
        {
            if (i + 1 == numpoints)
            {
                // We have reached the last point and need to connect it to the beginning
                perry += (float)(Math.Sqrt(Math.Pow((xcoord[0] - xcoord[i]),2) + Math.Pow((ycoord[0] - ycoord[i]), 2)));
            }
            else
            {
                perry += (float)(Math.Sqrt(Math.Pow((xcoord[i+1] - xcoord[i]),2) + 
                                           Math.Pow((ycoord[i+1] - ycoord[i]), 2)));
            }
        }
        
        return perry;
    }

    public float[] centroid()
    {
        float xcent = 0f;
        for (int i = 0; i < numpoints; i++)
        {
            xcent += xcoord[i];
        }
        xcent /= numpoints;
        
        float ycent = 0f;
        for (int i = 0; i < numpoints; i++)
        {
            ycent += ycoord[i];
        }
        ycent /= numpoints;
        float[] centroid = new float[2];
        centroid[0] = xcent;
        centroid[1] = ycent;

        return centroid;
    }

    /*
     * Function to build the point arrays of the polygon
     * param: values: the csv line extracted to array
     */
    public void BuildXYCoord(string[] values)
    {
        xcoord = new float[numpoints];
        ycoord = new float[numpoints];
        for (int i = 3; i+4 <= values.Length+1; i+=4)
        {
            xcoord[(i+1)/4-1] = (float)(Convert.ToDouble(values[i]));
        }

        for (int i = 5; i + 4 <= values.Length+4; i += 4)
        {
            ycoord[(i-1)/4-1] = (float)(Convert.ToDouble(values[i]));
        }
    }
}

class Uniform : Shape   // For Squares and Equilaterals
{
    private float sideLength;
    public int option;

    public float area()
    {
        if (option == 0)
        {
            return (float)(Math.Pow(sideLength, 2));
        }
        else
        {
            return (float)((Math.Sqrt(3) / 4) * Math.Pow(sideLength, 2));
        }
    }

    public float permimeter()
    {
        if (option == 0)
        {
            return (float)(sideLength*4);
        }
        else
        {
            return (float)(sideLength*3);
        }
    }

    public void setUp(float length, int opt)
    {
        sideLength = length;
        option = opt;
    }
}

class Program
{
    public static void Main(string[] args)
    {
        // using(var reader = new StreamReader(@"C:\Users\Rhino\RiderProjects\TestExecutable\ConsoleApp1\ShapeList2.csv"))
        using(var reader = new StreamReader(@"ShapeList2.csv"))
        {
            string path = "output.csv";
            
            if (File.Exists(path))  
            {  
                File.Delete(path);  
            } 
            using (FileStream file = File.Create(path)){}
            
            using (var fs = new StreamWriter(path))
            {
                var headers = string.Format("{0},{1},{2},{3},{4},{5},{6}", 
                    "ShapeID", "Shape", "Area", "Perimeter", "Centroid X", "Centroid Y", "Orientation");
                fs.WriteLine(headers);
                fs.Flush();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line!.Split(',');
                    
                    string shape = values[1];
                    float a = 0.0f;
                    float p = 0.0f;
                    float xcent = (float)(Convert.ToDouble(values[3]));
                    float ycent = (float)(Convert.ToDouble(values[5]));
                    float orient = 0f;
                    
                    switch (shape)
                    {
                        case "Circle":
                            Circle circle = new Circle();
                            circle.setUp((float)Convert.ToDouble(values[7]));
                            a = circle.area();
                            p = circle.permimeter();
                            break;
                        case "Ellipse":
                            Ellipse ellipse = new Ellipse();
                            ellipse.setUp((float)Convert.ToDouble(values[7]), (float)Convert.ToDouble(values[9]));
                            orient = (float)Convert.ToDouble(values[9]);
                            a = ellipse.area();
                            p = ellipse.permimeter();
                            break;
                        case "Polygon":
                            Polygon polygon = new Polygon();
                            polygon.SetSize(values.Length);
                            polygon.BuildXYCoord(values);
                            a = polygon.area()/2;
                            p = polygon.permimeter();
                            float[] polycent = polygon.centroid();
                            xcent = polycent[0];
                            ycent = polycent[1];
                            break;
                        case "Square":
                            Uniform square = new Uniform();
                            square.setUp((float)Convert.ToDouble(values[7]), 0);
                            orient = (float)Convert.ToDouble(values[9]);
                            a = square.area();
                            p = square.permimeter();
                            break;
                        case "Equilateral Triangle":
                            Uniform equilateral = new Uniform();
                            equilateral.setUp((float)Convert.ToDouble(values[7]), 1);
                            orient = (float)Convert.ToDouble(values[9]);
                            a = equilateral.area();
                            p = equilateral.permimeter();
                            break;
                    }

                    var shapeline = string.Format("{0},{1},{2},{3},{4},{5},{6}", 
                        values[0], values[1], a.ToString(), p.ToString(), xcent.ToString(), ycent.ToString(), orient.ToString());
                    fs.WriteLine(shapeline);
                    fs.Flush();
                }

            }
        }
    }
    
}
