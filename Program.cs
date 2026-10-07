
using Spectre.Console;

namespace TablaAmortizacion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            AnsiConsole.Write(
                new FigletText("Tabla de Amortización")
                    .Centered()
                    .Color(Color.Green));

            AnsiConsole.MarkupLine("[bold green]=== CALCULADORA DE TABLA DE AMORTIZACIÓN ===[/]\n");

            
            decimal monto = AnsiConsole.Prompt(
                new TextPrompt<decimal>("[bold yellow]Ingrese el monto del préstamo (M):[/]")
                    .ValidationErrorMessage("[red]Por favor, ingrese un monto válido mayor a 0.[/]")
                    .Validate(m => m > 0 ? ValidationResult.Success() : ValidationResult.Error()));

            decimal tasaAnual = AnsiConsole.Prompt(
                new TextPrompt<decimal>("[bold yellow]Ingrese la tasa de interés anual (%):[/]")
                    .ValidationErrorMessage("[red]La tasa de interés debe ser un valor positivo.[/]")
                    .Validate(t => t > 0 ? ValidationResult.Success() : ValidationResult.Error()));

            int meses = AnsiConsole.Prompt(
                new TextPrompt<int>("[bold yellow]Ingrese el plazo del préstamo en meses (n):[/]")
                    .ValidationErrorMessage("[red]El plazo debe ser al menos de 1 mes.[/]")
                    .Validate(n => n > 0 ? ValidationResult.Success() : ValidationResult.Error()));

            
            
            decimal tasaMensualDecimal = (tasaAnual / 12) / 100;
            double i = (double)tasaMensualDecimal;
            double n = meses;
            double M = (double)monto;

            double factor = Math.Pow(1 + i, n);
            double cuotaFijaDouble = M * (i * factor) / (factor - 1);
            decimal cuotaFija = Math.Round((decimal)cuotaFijaDouble, 2);

            
            AnsiConsole.WriteLine();
            var panelResumen = new Panel(
                $"[bold]Monto:[/] ${monto:N2}\n" +
                $"[bold]Tasa Mensual:[/] {tasaAnual / 12:F2}%\n" +
                $"[bold]Plazo:[/] {meses} meses\n" +
                $"[bold cyan]Cuota Fija Mensual:[/] ${cuotaFija:N2}")
            {
                Header = new PanelHeader("[bold cyan] Resumen del Préstamo [/]"),
                Padding = new Padding(1, 1, 1, 1)
            };
            AnsiConsole.Write(panelResumen);
            AnsiConsole.WriteLine();

            
            var tabla = new Table();
            tabla.Border(TableBorder.Rounded);
            tabla.Title("[bold blue]TABLA DE AMORTIZACIÓN DE DEUDA[/]");

            // Agregar columnas a la tabla
            tabla.AddColumn(new TableColumn("[bold]No. Cuota[/]").Centered());
            tabla.AddColumn(new TableColumn("[bold]Pago de Cuota[/]").RightAligned());
            tabla.AddColumn(new TableColumn("[bold]Interés a Pagar[/]").RightAligned());
            tabla.AddColumn(new TableColumn("[bold]Abono a Capital[/]").RightAligned());
            tabla.AddColumn(new TableColumn("[bold]Saldo Pendiente[/]").RightAligned());

            decimal saldo = monto;

            
            for (int periodo = 1; periodo <= meses; periodo++)
            {
               
                decimal interesPagar = Math.Round(saldo * tasaMensualDecimal, 2);

                
                decimal abonoCapital = Math.Round(cuotaFija - interesPagar, 2);

                
                if (periodo == meses)
                {
                    abonoCapital = saldo;
                    cuotaFija = abonoCapital + interesPagar;
                    saldo = 0.00m;
                }
                else
                {
                    saldo = Math.Round(saldo - abonoCapital, 2);
                }

            
                tabla.AddRow(
                    periodo.ToString(),
                    $"${cuotaFija:N2}",
                    $"${interesPagar:N2}",
                    $"${abonoCapital:N2}",
                    $"${saldo:N2}"
                );
            }

            
            AnsiConsole.Write(tabla);
            AnsiConsole.MarkupLine("\n[bold green]✓ Cálculo completado con éxito.[/]");
        }
    }
}