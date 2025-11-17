using hinos_api.Models;

namespace hinos_api.Data;

public static class DbSeeder
{
    public static void Seed(HymnsDbContext context)
    {
        if (context.Hymns.Any())
        {
            return; // DB já foi populado
        }

        var hymns = new List<Hymn>
        {
            new Hymn
            {
                Number = "101",
                Title = "Graça Maravilhosa",
                Category = "hinario",
                HymnBook = "Hinário Adventista do Sétimo Dia",
                Key = "G",
                Bpm = 72,
                Verses = new List<Verse>
                {
                    new Verse { Type = "V1", Lines = new List<string> { "Graça excelsa, doce som", "Que um pecador salvou", "Eu, perdido me encontrei", "Mas agora salvo sou" } },
                    new Verse { Type = "R", Lines = new List<string> { "Fui cego mas hoje vejo já", "Sua luz a brilhar em mim", "Graça excelsa sem igual", "Me salvou, me salvou enfim" } },
                    new Verse { Type = "V2", Lines = new List<string> { "Foi a graça que ao meu coração", "O temor ensinou", "E esse medo ela afastou", "Quando nela eu confiou" } },
                    new Verse { Type = "V3", Lines = new List<string> { "Tantos perigos já passei", "Nesta vida aqui", "Mas foi a graça que me trouxe", "E me guiará até o fim" } }
                }
            },
            new Hymn
            {
                Number = "S12",
                Title = "Vitória em Jesus",
                Category = "suplementar",
                HymnBook = "Suplementar Adventista",
                Key = "D",
                Bpm = 84,
                Verses = new List<Verse>
                {
                    new Verse { Type = "V1", Lines = new List<string> { "Em Jesus temos a vitória", "Ele nos dá poder", "Com Seu nome e Sua glória", "Vamos sempre vencer" } },
                    new Verse { Type = "R", Lines = new List<string> { "Vitória, vitória", "Em Jesus há vitória", "Sobre o mal, sobre a dor", "Cristo é nosso Salvador" } },
                    new Verse { Type = "V2", Lines = new List<string> { "Nada pode nos separar", "Do amor de Deus", "Sua graça vem nos guardar", "Nos leva para os céus" } }
                }
            },
            new Hymn
            {
                Number = "150",
                Title = "Castelo Forte",
                Category = "hinario",
                HymnBook = "Hinário Adventista do Sétimo Dia",
                Key = "C",
                Bpm = 76,
                Verses = new List<Verse>
                {
                    new Verse { Type = "V1", Lines = new List<string> { "Castelo forte é nosso Deus", "Espada e bom escudo", "Com Seu poder defende os Seus", "Em todo transe agudo" } },
                    new Verse { Type = "V2", Lines = new List<string> { "Com nossa força nada hás", "Sozinhos perderíamos", "Mas nosso Deus socorro traz", "E certo venceríamos" } },
                    new Verse { Type = "V3", Lines = new List<string> { "Se nos quiseram devorar", "Demônios não contamos", "Nada nos pode dominar", "Pois Cristo nos guarda" } }
                }
            },
            new Hymn
            {
                Number = "200",
                Title = "Lindo País",
                Category = "hinario",
                HymnBook = "Hinário Adventista do Sétimo Dia",
                Key = "F",
                Bpm = 68,
                Verses = new List<Verse>
                {
                    new Verse { Type = "V1", Lines = new List<string> { "Lindo país de luz sem par", "Lar dos remidos do Senhor", "Desejo muito lá morar", "Na santa terra de esplendor" } },
                    new Verse { Type = "R", Lines = new List<string> { "Lar feliz, lar feliz", "Além do azul céu de luz", "Lar feliz, lar feliz", "Preparado por Jesus" } },
                    new Verse { Type = "V2", Lines = new List<string> { "Nosso Mestre foi preparar", "Mansões de paz e de amor", "E promete nos levar", "Para esse reino de fulgor" } }
                }
            },
            new Hymn
            {
                Number = "N5",
                Title = "Aleluia ao Cordeiro",
                Category = "novos",
                HymnBook = "Novos Cânticos",
                Key = "A",
                Bpm = 95,
                Verses = new List<Verse>
                {
                    new Verse { Type = "V1", Lines = new List<string> { "Aleluia, aleluia", "Ao Cordeiro de Deus", "Que morreu por mim", "E me levou aos céus" } },
                    new Verse { Type = "C", Lines = new List<string> { "Santo, Santo", "É o Senhor", "Digno de receber", "Glória e louvor" } },
                    new Verse { Type = "Ponte", Lines = new List<string> { "Levantamos nossas vozes", "Para Te adorar", "És o Rei dos reis", "Vamos Te exaltar" } }
                }
            },
            new Hymn
            {
                Number = "C45",
                Title = "Quando em Glória",
                Category = "canticos",
                HymnBook = "Cânticos Laicos",
                Key = "E",
                Bpm = 70,
                Verses = new List<Verse>
                {
                    new Verse { Type = "V1", Lines = new List<string> { "Quando em glória Cristo vier", "Para buscar os Seus", "Os remidos vão subir", "Ao encontro nos céus" } },
                    new Verse { Type = "R", Lines = new List<string> { "Que dia será", "Quando eu vir Seu rosto", "E contemplar", "A glória de meu Rei" } }
                }
            },
            new Hymn
            {
                Number = "1",
                Title = "Deus é Nosso Refúgio",
                Category = "hinario",
                HymnBook = "Hinário Adventista do Sétimo Dia",
                Key = "G",
                Bpm = 80,
                Verses = new List<Verse>
                {
                    new Verse { Type = "V1", Lines = new List<string> { "Deus é nosso refúgio", "Nossa força e vigor", "Amparo seguro", "Em toda tribulação" } },
                    new Verse { Type = "V2", Lines = new List<string> { "Não tememos por isso", "Ainda que a terra se mova", "E os montes se lancem", "Ao mar em furor" } }
                }
            },
            new Hymn
            {
                Number = "S1",
                Title = "Coração Agradecido",
                Category = "suplementar",
                HymnBook = "Suplementar Adventista",
                Key = "C",
                Bpm = 78,
                Verses = new List<Verse>
                {
                    new Verse { Type = "V1", Lines = new List<string> { "Com um coração agradecido", "Venho Te louvar", "Por tudo que tens feito", "Quero Te adorar" } },
                    new Verse { Type = "R", Lines = new List<string> { "Graças Te dou, Senhor", "Por Teu amor", "Graças Te dou", "Meu Salvador" } }
                }
            },
            new Hymn
            {
                Number = "N1",
                Title = "Fonte de Água Viva",
                Category = "novos",
                HymnBook = "Novos Cânticos",
                Key = "D",
                Bpm = 88,
                Verses = new List<Verse>
                {
                    new Verse { Type = "V1", Lines = new List<string> { "Tu és fonte de água viva", "Que sacia minha sede", "Em Ti encontro vida", "E paz que me completa" } },
                    new Verse { Type = "C", Lines = new List<string> { "Vem fluir em mim", "Tua presença", "Vem me encher", "Com Tua essência" } }
                }
            },
            new Hymn
            {
                Number = "250",
                Title = "Fiel é o Senhor",
                Category = "hinario",
                HymnBook = "Hinário Adventista do Sétimo Dia",
                Key = "A",
                Bpm = 75,
                Verses = new List<Verse>
                {
                    new Verse { Type = "V1", Lines = new List<string> { "Fiel é o Senhor que nos guarda", "Seu amor nunca falhará", "Nas promessas que nos fez", "Sempre podemos confiar" } },
                    new Verse { Type = "R", Lines = new List<string> { "Fiel, fiel é nosso Deus", "Seu amor é eternal", "Fiel, sempre fiel", "Para sempre reinará" } }
                }
            }
        };

        context.Hymns.AddRange(hymns);
        context.SaveChanges();

        // Atualizar HymnId em todos os versos após salvar
        foreach (var hymn in hymns)
        {
            foreach (var verse in hymn.Verses)
            {
                verse.HymnId = hymn.Id;
            }
        }
        context.SaveChanges();
    }
}
