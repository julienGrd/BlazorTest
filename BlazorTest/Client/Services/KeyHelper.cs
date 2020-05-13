using System;
using System.Linq;

namespace BlazorTest.Client.Services
{
    public static class KeyHelper
    {
        public static Key? GetKeyFromString(string key)
        {
            var list = EnumExtensions.GetValuesWithDescription<Key>();

            //il faut faire un petit traitement pour les touches numériques
            if (int.TryParse(key, out int decimalKey))
            {
                key = $"NumPad{key}";
            }
            else if (key.StartsWith("Arrow"))
            {
                key = key.Remove(0, 5);
            }

            if (!string.IsNullOrEmpty(key))
            {
                var obj = list.Where(x => x.Value.Equals(key, StringComparison.InvariantCultureIgnoreCase));
                if (obj.Any())
                {
                    return obj.First().Key;
                }
            }
            return null;
        }
    }

    // Résumé :
    //     Spécifie les valeurs de clés possibles sur un clavier.
    public enum Key
    {
        // Résumé :
        //     Valeur spéciale indiquant l'absence de touche.
        None = 0,
        //
        // Résumé :
        //     Touche RET. ARR (RETOUR ARRIÈRE).
        Back = 1,
        //
        // Résumé :
        //     Touche TAB (TABULATION).
        Tab = 2,
        //
        // Résumé :
        //     Touche ENTRÉE.
        Enter = 3,
        //
        // Résumé :
        //     Touche MAJ (MAJUSCULE).
        Shift = 4,
        //
        // Résumé :
        //     Touche CTRL (CONTRÔLE).
        Ctrl = 5,
        //
        // Résumé :
        //     Touche ALT.
        Alt = 6,
        //
        // Résumé :
        //     Touche VERR. MAJ (VERROUILLAGE MAJUSCULE).
        CapsLock = 7,
        //
        // Résumé :
        //     Touche ÉCHAP (ÉCHAPPEMENT).
        Escape = 8,
        //
        // Résumé :
        //     Touche ESPACE.
        Space = 9,
        //
        // Résumé :
        //     Touche PG. PRÉC (PAGE PRÉCÉDENTE).
        PageUp = 10,
        //
        // Résumé :
        //     Touche PG. SUIV (PAGE SUIVANTE).
        PageDown = 11,
        //
        // Résumé :
        //     Touche FIN.
        End = 12,
        //
        // Résumé :
        //     Touche DÉBUT.
        Home = 13,
        //
        // Résumé :
        //     Touche Gauche.
        Left = 14,
        //
        // Résumé :
        //     Touche HAUT.
        Up = 15,
        //
        // Résumé :
        //     Touche DROITE.
        Right = 16,
        //
        // Résumé :
        //     Touche Bas.
        Down = 17,
        //
        // Résumé :
        //     Touche INSER (INSERTION).
        Insert = 18,
        //
        // Résumé :
        //     Touche SUPPR (SUPPRESSION).
        Delete = 19,
        //
        // Résumé :
        //     Touche 0 (zéro).
        D0 = 20,
        //
        // Résumé :
        //     Touche 1.
        D1 = 21,
        //
        // Résumé :
        //     Touche 2.
        D2 = 22,
        //
        // Résumé :
        //     Touche 3.
        D3 = 23,
        //
        // Résumé :
        //     Touche 4.
        D4 = 24,
        //
        // Résumé :
        //     Touche 5.
        D5 = 25,
        //
        // Résumé :
        //     Touche 6.
        D6 = 26,
        //
        // Résumé :
        //     Touche 7.
        D7 = 27,
        //
        // Résumé :
        //     Touche 8.
        D8 = 28,
        //
        // Résumé :
        //     Touche 9.
        D9 = 29,
        //
        // Résumé :
        //     Touche A.
        A = 30,
        //
        // Résumé :
        //     Touche B.
        B = 31,
        //
        // Résumé :
        //     Touche C.
        C = 32,
        //
        // Résumé :
        //     Touche D.
        D = 33,
        //
        // Résumé :
        //     Touche E.
        E = 34,
        //
        // Résumé :
        //     Touche F.
        F = 35,
        //
        // Résumé :
        //     Touche G.
        G = 36,
        //
        // Résumé :
        //     Touche H.
        H = 37,
        //
        // Résumé :
        //     Touche I.
        I = 38,
        //
        // Résumé :
        //     Touche J.
        J = 39,
        //
        // Résumé :
        //     Touche K.
        K = 40,
        //
        // Résumé :
        //     Touche L.
        L = 41,
        //
        // Résumé :
        //     Touche M.
        M = 42,
        //
        // Résumé :
        //     Touche N.
        N = 43,
        //
        // Résumé :
        //     Touche O.
        O = 44,
        //
        // Résumé :
        //     Touche P.
        P = 45,
        //
        // Résumé :
        //     Touche Q.
        Q = 46,
        //
        // Résumé :
        //     Touche R.
        R = 47,
        //
        // Résumé :
        //     Touche S.
        S = 48,
        //
        // Résumé :
        //     Touche T.
        T = 49,
        //
        // Résumé :
        //     Touche U.
        U = 50,
        //
        // Résumé :
        //     Touche V.
        V = 51,
        //
        // Résumé :
        //     Touche W.
        W = 52,
        //
        // Résumé :
        //     Touche X.
        X = 53,
        //
        // Résumé :
        //     Touche Y.
        Y = 54,
        //
        // Résumé :
        //     Touche Z.
        Z = 55,
        //
        // Résumé :
        //     Touche F1.
        F1 = 56,
        //
        // Résumé :
        //     Touche F2.
        F2 = 57,
        //
        // Résumé :
        //     Touche F3.
        F3 = 58,
        //
        // Résumé :
        //     La touche F4.
        F4 = 59,
        //
        // Résumé :
        //     La touche F5.
        F5 = 60,
        //
        // Résumé :
        //     La touche F6.
        F6 = 61,
        //
        // Résumé :
        //     La touche F7.
        F7 = 62,
        //
        // Résumé :
        //     Touche F8.
        F8 = 63,
        //
        // Résumé :
        //     Touche F9.
        F9 = 64,
        //
        // Résumé :
        //     Touche F10.
        F10 = 65,
        //
        // Résumé :
        //     Touche F11.
        F11 = 66,
        //
        // Résumé :
        //     Touche F12.
        F12 = 67,
        //
        // Résumé :
        //     Touche 0 du pavé numérique.
        NumPad0 = 68,
        //
        // Résumé :
        //     Touche 1 du pavé numérique.
        NumPad1 = 69,
        //
        // Résumé :
        //     Touche 2 du pavé numérique.
        NumPad2 = 70,
        //
        // Résumé :
        //     Touche 3 du pavé numérique.
        NumPad3 = 71,
        //
        // Résumé :
        //     Touche 4 du pavé numérique.
        NumPad4 = 72,
        //
        // Résumé :
        //     Touche 5 du pavé numérique.
        NumPad5 = 73,
        //
        // Résumé :
        //     Touche 6 du pavé numérique.
        NumPad6 = 74,
        //
        // Résumé :
        //     Touche 7 du pavé numérique.
        NumPad7 = 75,
        //
        // Résumé :
        //     Touche 8 du pavé numérique.
        NumPad8 = 76,
        //
        // Résumé :
        //     Touche 9 du pavé numérique.
        NumPad9 = 77,
        //
        // Résumé :
        //     Touche * (MULTIPLICATION).
        Multiply = 78,
        //
        // Résumé :
        //     Touche + (ADDITION).
        Add = 79,
        //
        // Résumé :
        //     Touche – (SOUSTRACTION).
        Subtract = 80,
        //
        // Résumé :
        //     Touche .(DÉCIMALE).
        Decimal = 81,
        //
        // Résumé :
        //     Touche / (DIVISION).
        Divide = 82,
        //
        // Résumé :
        //     Valeur spéciale indiquant que la touche est en dehors des limites de cette
        //     énumération.
        Unknown = 255,
    }
}
