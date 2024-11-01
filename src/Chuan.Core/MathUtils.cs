// ReSharper disable All
// ReSharper disable InvalidXmlDocComment

using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;

namespace Chuan.Core;

public static class MathUtils
{
    /// <summary>
    /// 线性回归
    ///
    /// 用在短期的预估中
    /// </summary>
    /// <param name="x">time</param>
    /// <param name="y">value</param>
    /// <returns>
    /// y = sx + i
    /// </returns>
    public static (double sloope, double intercept) LinearRegression(double[] x, double[] y)
        => Fit.Line(x, y);

    /// <summary>
    /// 用在长期的触顶判断
    /// 
    /// 用来判断整个项目的健康状态
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="tolerance">拟合的误差容忍度</param>
    /// <param name="iteration">拟合最大次数</param>
    /// <returns></returns>
    public static (double L, double k, double t0)? LogisticRegression(
        double[] x,
        double[] y,
        double tolerance = 0.0000001,
        int iteration = 20000)
    {
        var optimizer = new NelderMeadSimplex(tolerance, iteration);

        var coefficients = Fit.Polynomial(x, y, 2);

        /**
         * f(t) = \frac{L}{1 + e^{-k(t - t_0)}}
         * - f(t) 是在时间 t 时的函数值（例如，人口数量或药物浓度）。
         * - L 是曲线的上限，代表最大值或饱和值。
         * - k 是曲线的增长速率，决定了增长的快慢。
         * - t_0 是曲线的中点，表示函数值达到一半最大值时的时间。
         * - e 是自然对数的底数（约等于2.71828）。
         */
        var result = optimizer.FindMinimum(ObjectiveFunction.Value(args =>
        {
            var L = args[1];
            var k = args[0];
            var t0 = args[2];

            var cals = x.Select(item => LogisticPredict(L, k, t0, item));
            var variance = cals.Select((item, i) => Math.Pow(item - y[i], 2));

            var sum = variance.Sum();

            Console.WriteLine($"{L,16} - {k,16} - {t0,16} - {sum,16}");
            return sum;

        }), new DenseVector([
            coefficients[1],
            coefficients[2],        // 
            (x.Min() + x.Max()) / 2
        ]));

        var paras = result.MinimizingPoint.ToArray();
        if (paras is null) return null;

        return (paras[0], paras[1], paras[2]);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="L"></param>
    /// <param name="k"></param>
    /// <param name="t0"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public static double LogisticPredict(double L, double k, double t0, double x)
        => L / (1 + Math.Exp(-k * (x - t0)));
}