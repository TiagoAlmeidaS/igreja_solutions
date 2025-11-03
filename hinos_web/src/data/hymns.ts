import { Hymn } from '../types/hymn';

// NOTA: Estes são dados de exemplo para demonstração.
// Para produção, substitua por letras reais com permissões apropriadas.
export const hymnsDatabase: Hymn[] = [
  {
    id: '101',
    number: '101',
    title: 'Graça Maravilhosa',
    category: 'hinario',
    hymnBook: 'Hinário Adventista do Sétimo Dia',
    key: 'G',
    bpm: 72,
    verses: [
      {
        type: 'V1',
        lines: [
          'Graça excelsa, doce som',
          'Que um pecador salvou',
          'Eu, perdido me encontrei',
          'Mas agora salvo sou'
        ]
      },
      {
        type: 'R',
        lines: [
          'Fui cego mas hoje vejo já',
          'Sua luz a brilhar em mim',
          'Graça excelsa sem igual',
          'Me salvou, me salvou enfim'
        ]
      },
      {
        type: 'V2',
        lines: [
          'Foi a graça que ao meu coração',
          'O temor ensinou',
          'E esse medo ela afastou',
          'Quando nela eu confiou'
        ]
      },
      {
        type: 'V3',
        lines: [
          'Tantos perigos já passei',
          'Nesta vida aqui',
          'Mas foi a graça que me trouxe',
          'E me guiará até o fim'
        ]
      }
    ]
  },
  {
    id: 'S12',
    number: 'S12',
    title: 'Vitória em Jesus',
    category: 'suplementar',
    hymnBook: 'Suplementar Adventista',
    key: 'D',
    bpm: 84,
    verses: [
      {
        type: 'V1',
        lines: [
          'Em Jesus temos a vitória',
          'Ele nos dá poder',
          'Com Seu nome e Sua glória',
          'Vamos sempre vencer'
        ]
      },
      {
        type: 'R',
        lines: [
          'Vitória, vitória',
          'Em Jesus há vitória',
          'Sobre o mal, sobre a dor',
          'Cristo é nosso Salvador'
        ]
      },
      {
        type: 'V2',
        lines: [
          'Nada pode nos separar',
          'Do amor de Deus',
          'Sua graça vem nos guardar',
          'Nos leva para os céus'
        ]
      }
    ]
  },
  {
    id: '150',
    number: '150',
    title: 'Castelo Forte',
    category: 'hinario',
    hymnBook: 'Hinário Adventista do Sétimo Dia',
    key: 'C',
    bpm: 76,
    verses: [
      {
        type: 'V1',
        lines: [
          'Castelo forte é nosso Deus',
          'Espada e bom escudo',
          'Com Seu poder defende os Seus',
          'Em todo transe agudo'
        ]
      },
      {
        type: 'V2',
        lines: [
          'Com nossa força nada hás',
          'Sozinhos perderíamos',
          'Mas nosso Deus socorro traz',
          'E certo venceríamos'
        ]
      },
      {
        type: 'V3',
        lines: [
          'Se nos quiseram devorar',
          'Demônios não contamos',
          'Nada nos pode dominar',
          'Pois Cristo nos guarda'
        ]
      }
    ]
  },
  {
    id: '200',
    number: '200',
    title: 'Lindo País',
    category: 'hinario',
    hymnBook: 'Hinário Adventista do Sétimo Dia',
    key: 'F',
    bpm: 68,
    verses: [
      {
        type: 'V1',
        lines: [
          'Lindo país de luz sem par',
          'Lar dos remidos do Senhor',
          'Desejo muito lá morar',
          'Na santa terra de esplendor'
        ]
      },
      {
        type: 'R',
        lines: [
          'Lar feliz, lar feliz',
          'Além do azul céu de luz',
          'Lar feliz, lar feliz',
          'Preparado por Jesus'
        ]
      },
      {
        type: 'V2',
        lines: [
          'Nosso Mestre foi preparar',
          'Mansões de paz e de amor',
          'E promete nos levar',
          'Para esse reino de fulgor'
        ]
      }
    ]
  },
  {
    id: 'N5',
    number: 'N5',
    title: 'Aleluia ao Cordeiro',
    category: 'novos',
    hymnBook: 'Novos Cânticos',
    key: 'A',
    bpm: 95,
    verses: [
      {
        type: 'V1',
        lines: [
          'Aleluia, aleluia',
          'Ao Cordeiro de Deus',
          'Que morreu por mim',
          'E me levou aos céus'
        ]
      },
      {
        type: 'C',
        lines: [
          'Santo, Santo',
          'É o Senhor',
          'Digno de receber',
          'Glória e louvor'
        ]
      },
      {
        type: 'Ponte',
        lines: [
          'Levantamos nossas vozes',
          'Para Te adorar',
          'És o Rei dos reis',
          'Vamos Te exaltar'
        ]
      }
    ]
  },
  {
    id: 'C45',
    number: 'C45',
    title: 'Quando em Glória',
    category: 'canticos',
    hymnBook: 'Cânticos Laicos',
    key: 'E',
    bpm: 70,
    verses: [
      {
        type: 'V1',
        lines: [
          'Quando em glória Cristo vier',
          'Para buscar os Seus',
          'Os remidos vão subir',
          'Ao encontro nos céus'
        ]
      },
      {
        type: 'R',
        lines: [
          'Que dia será',
          'Quando eu vir Seu rosto',
          'E contemplar',
          'A glória de meu Rei'
        ]
      }
    ]
  },
  {
    id: '1',
    number: '1',
    title: 'Deus é Nosso Refúgio',
    category: 'hinario',
    hymnBook: 'Hinário Adventista do Sétimo Dia',
    key: 'G',
    bpm: 80,
    verses: [
      {
        type: 'V1',
        lines: [
          'Deus é nosso refúgio',
          'Nossa força e vigor',
          'Amparo seguro',
          'Em toda tribulação'
        ]
      },
      {
        type: 'V2',
        lines: [
          'Não tememos por isso',
          'Ainda que a terra se mova',
          'E os montes se lancem',
          'Ao mar em furor'
        ]
      }
    ]
  },
  {
    id: 'S1',
    number: 'S1',
    title: 'Coração Agradecido',
    category: 'suplementar',
    hymnBook: 'Suplementar Adventista',
    key: 'C',
    bpm: 78,
    verses: [
      {
        type: 'V1',
        lines: [
          'Com um coração agradecido',
          'Venho Te louvar',
          'Por tudo que tens feito',
          'Quero Te adorar'
        ]
      },
      {
        type: 'R',
        lines: [
          'Graças Te dou, Senhor',
          'Por Teu amor',
          'Graças Te dou',
          'Meu Salvador'
        ]
      }
    ]
  },
  {
    id: 'N1',
    number: 'N1',
    title: 'Fonte de Água Viva',
    category: 'novos',
    hymnBook: 'Novos Cânticos',
    key: 'D',
    bpm: 88,
    verses: [
      {
        type: 'V1',
        lines: [
          'Tu és fonte de água viva',
          'Que sacia minha sede',
          'Em Ti encontro vida',
          'E paz que me completa'
        ]
      },
      {
        type: 'C',
        lines: [
          'Vem fluir em mim',
          'Tua presença',
          'Vem me encher',
          'Com Tua essência'
        ]
      }
    ]
  },
  {
    id: '250',
    number: '250',
    title: 'Fiel é o Senhor',
    category: 'hinario',
    hymnBook: 'Hinário Adventista do Sétimo Dia',
    key: 'A',
    bpm: 75,
    verses: [
      {
        type: 'V1',
        lines: [
          'Fiel é o Senhor que nos guarda',
          'Seu amor nunca falhará',
          'Nas promessas que nos fez',
          'Sempre podemos confiar'
        ]
      },
      {
        type: 'R',
        lines: [
          'Fiel, fiel é nosso Deus',
          'Seu amor é eternal',
          'Fiel, sempre fiel',
          'Para sempre reinará'
        ]
      }
    ]
  }
];
